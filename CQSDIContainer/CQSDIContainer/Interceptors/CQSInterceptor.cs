using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public abstract class CQSInterceptorBasic : IInterceptor
	{
		protected abstract void InterceptSync(IInvocation invocation);
		protected abstract void InterceptAsync(IInvocation invocation);
		
		public void Intercept(IInvocation invocation)
		{
			if (IsAsyncMethod(invocation.Method))
				InterceptAsync(invocation);
			else
				InterceptSync(invocation);
		}

		private static bool IsAsyncMethod(MethodInfo method)
		{
			return method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
		}
	}

	public abstract class CQSInterceptor : IInterceptor
	{
		private static readonly MethodInfo _handleAsyncMethodInfo = typeof(CQSInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Instance | BindingFlags.NonPublic);

		public void Intercept(IInvocation invocation)
		{
			var delegateType = GetDelegateType(invocation);
			switch (delegateType)
			{
				case MethodType.Synchronous:
					HandleSynchronousInvocation(invocation.Proceed);
					break;
				
				case MethodType.AsynchronousAction:
					invocation.Proceed();
					invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue);
					break;

				case MethodType.AsynchronousFunction:
					invocation.Proceed();
					ExecuteHandleAsyncWithResultUsingReflection(invocation);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected abstract void HandleSynchronousInvocation(Action synchronousInvoker);
		protected abstract Task HandleInvocationOfAsynchronousAction(Func<Task> awaitableInvoker);
		protected abstract Task<TReturnType> HandleInvocationOfAsynchronousFunction<TReturnType>(Func<Task<TReturnType>> awaitableInvoker);

		private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var mi = _handleAsyncMethodInfo.MakeGenericMethod(resultType);
			invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue });
		}

		private async Task HandleAsync(Task task)
		{
			await HandleInvocationOfAsynchronousAction(async () => await task);
		}

		private async Task<TReturnType> HandleAsyncWithResult<TReturnType>(Task<TReturnType> task)
		{
			return await HandleInvocationOfAsynchronousFunction(async () => await task);
		}

		private static MethodType GetDelegateType(IInvocation invocation)
		{
			var returnType = invocation.Method.ReturnType;
			if (returnType == typeof(Task))
				return MethodType.AsynchronousAction;

			if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
				return MethodType.AsynchronousFunction;

			return MethodType.Synchronous;
		}

		private enum MethodType
		{
			Synchronous,
			AsynchronousAction,
			AsynchronousFunction
		}
	}
}
