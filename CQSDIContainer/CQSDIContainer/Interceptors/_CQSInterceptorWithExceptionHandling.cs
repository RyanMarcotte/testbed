using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;

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
		/// Called immediately after receiving a return value from the invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The return value received.</param>
		protected virtual void OnReceiveReturnValueFromInvocation(ComponentModel componentModel, object returnValue) { }

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
				OnReceiveReturnValueFromInvocation(componentModel, invocation.ReturnValue);
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
				OnReceiveReturnValueFromInvocation(componentModel, invocation.ReturnValue);
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
