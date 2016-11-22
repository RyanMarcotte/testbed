using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Exceptions;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Interceptors
{
	/// <summary>
	/// Base class for interceptors applied to CQS handlers that work around exception handling logic.
	/// </summary>
	public abstract class CQSInterceptorWithExceptionHandling : CQSInterceptor
	{
		/// <summary>
		/// Called just before beginning handler invocation.  Use for setup.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnBeginInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous query handler invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromQueryHandlerInvocation(ComponentModel componentModel, object returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous query handler invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(ComponentModel componentModel, object returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous command handler invocation that does not return any value.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnReceiveReturnValueFromCommandHandlerInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an synchronous command handler that returns a result.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler that returns a result.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue) { }

		/// <summary>
		/// Always called just before returning control to the caller.  Use for teardown.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnEndInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called when an exception has been thrown during invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="ex">The exception.</param>
		protected virtual void OnException(ComponentModel componentModel, Exception ex) { }

		#region Sealed Implementations

		protected sealed override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			try
			{
				OnBeginInvocation(componentModel);
				invocation.Proceed();
				OnReceiveReturnValueFromSynchronousMethodInvocation(invocation, componentModel);
			}
			catch (Exception ex)
			{
				OnException(componentModel, ex);
				throw;
			}
			finally
			{
				OnEndInvocation(componentModel);
			}
		}

		protected sealed override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
		{
			try
			{
				OnBeginInvocation(componentModel);
				invocation.Proceed();
				switch (methodType)
				{
					case AsynchronousMethodType.Action:
						HandleAsync((Task)invocation.ReturnValue).GetAwaiter().GetResult();
						break;

					case AsynchronousMethodType.Function:
						ExecuteHandleAsyncWithResultUsingReflection(invocation);
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Invalid value!!");
				}
				OnReceiveReturnValueFromAsynchronousMethodInvocation(invocation, componentModel);
			}
			catch (Exception ex)
			{
				OnException(componentModel, ex);
				throw;
			}
			finally
			{
				OnEndInvocation(componentModel);
			}
		}

		/// <summary>
		/// Called immediately after successfully completing the synchronous invocation.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		private void OnReceiveReturnValueFromSynchronousMethodInvocation(IInvocation invocation, ComponentModel componentModel)
		{
			if (!componentModel.Implementation.IsGenericType)
				throw new UnrecognizedCQSHandlerTypeException(componentModel);

			var handlerType = componentModel.Implementation.GetGenericTypeDefinition();
			if (handlerType == typeof(IQueryHandler<,>))
				OnReceiveReturnValueFromQueryHandlerInvocation(componentModel, invocation.ReturnValue);
			else if (handlerType == typeof(ICommandHandler<>))
				OnReceiveReturnValueFromCommandHandlerInvocation(componentModel);
			else if (handlerType == typeof(IResultCommandHandler<,>))
				ExecuteOnReceiveReturnValueFromResultCommandHandlerInvocationUsingReflection(invocation, componentModel);
			else
				throw new UnrecognizedCQSHandlerTypeException(componentModel);
		}

		#region OnReceiveReturnValueFromSynchronousMethodInvocation helpers

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> _onReceiveReturnValueFromSynchronousFunctionInvocationMethodLookup = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
		private static readonly MethodInfo _onReceiveReturnValueFromSynchronousFunctionInvocationMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(OnReceiveReturnValueFromResultCommandHandlerInvocation), BindingFlags.Instance | BindingFlags.NonPublic);

		private void ExecuteOnReceiveReturnValueFromResultCommandHandlerInvocationUsingReflection(IInvocation invocation, ComponentModel componentModel)
		{
			var successResultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var failureResultType = invocation.Method.ReturnType.GetGenericArguments()[1];
			var methodInfo = _onReceiveReturnValueFromSynchronousFunctionInvocationMethodLookup.GetOrAdd(Tuple.Create(successResultType, failureResultType), _onReceiveReturnValueFromSynchronousFunctionInvocationMethodInfo.MakeGenericMethod(successResultType, failureResultType));
			invocation.ReturnValue = methodInfo.Invoke(this, new[] { componentModel, invocation.ReturnValue });
		}

		#endregion

		/// <summary>
		/// Called immediately after successfully completing the asynchronous invocation.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		private void OnReceiveReturnValueFromAsynchronousMethodInvocation(IInvocation invocation, ComponentModel componentModel)
		{
			if (!componentModel.Implementation.IsGenericType)
				throw new UnrecognizedCQSHandlerTypeException(componentModel);

			var handlerType = componentModel.Implementation.GetGenericTypeDefinition();
			if (handlerType == typeof(IAsyncQueryHandler<,>))
				OnReceiveReturnValueFromAsyncQueryHandlerInvocation(componentModel, ((dynamic)invocation.ReturnValue).Result);
			else if (handlerType == typeof(IAsyncCommandHandler<>))
				OnReceiveReturnValueFromAsyncCommandHandlerInvocation(componentModel);
			else if (handlerType == typeof(IAsyncResultCommandHandler<,>))
				ExecuteOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationUsingReflection(invocation, componentModel);
			else
				throw new UnrecognizedCQSHandlerTypeException(componentModel);
		}

		#region OnReceiveReturnValueFromAsynchronousMethodInvocation helpers

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodLookup = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
		private static readonly MethodInfo _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation), BindingFlags.Instance | BindingFlags.NonPublic);
		
		private void ExecuteOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationUsingReflection(IInvocation invocation, ComponentModel componentModel)
		{
			var taskType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var successResultType = taskType.GetGenericArguments()[0];
			var failureResultType = taskType.GetGenericArguments()[1];
			var methodInfo = _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodLookup.GetOrAdd(Tuple.Create(successResultType, failureResultType), _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodInfo.MakeGenericMethod(successResultType, failureResultType));
			invocation.ReturnValue = methodInfo.Invoke(this, new object[] { componentModel, ((dynamic)invocation.ReturnValue).Result });
		}

		#endregion

		#region Enums

		protected enum HandlerType
		{
			Synchronous
		}
		#endregion

		#region Helper Methods (for async)

		private static async Task HandleAsync(Task task)
		{
			await task;
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task)
		{
			return await task;
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericHandleAsyncWithResultMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncWithResultMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericHandleAsyncWithResultMethodLookup.GetOrAdd(resultType, _handleAsyncWithResultMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(this, new[] { invocation.ReturnValue });
		}

		#endregion

		#endregion
	}
}
