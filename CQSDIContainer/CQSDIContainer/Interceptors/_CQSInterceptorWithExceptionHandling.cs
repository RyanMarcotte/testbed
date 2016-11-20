using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;

namespace CQSDIContainer.Interceptors
{
	/// <summary>
	/// Base class for interceptors applied to CQS handlers that work around exception handling logic.
	/// </summary>
	public abstract class CQSInterceptorWithExceptionHandling : CQSInterceptor
	{
		protected sealed override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			try
			{
				OnBeginInvocation(componentModel);
				invocation.Proceed();
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
						invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, componentModel);
						break;

					case AsynchronousMethodType.Function:
						ExecuteHandleAsyncWithResultUsingReflection(invocation);
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Invalid value!!");
				}
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

		protected virtual void OnBeginInvocation(ComponentModel componentModel) { }
		protected virtual void OnEndInvocation(ComponentModel componentModel) { }
		protected virtual void OnException(ComponentModel componentModel, Exception ex) { }

		private async Task HandleAsync(Task task, ComponentModel componentModel)
		{
			try
			{
				OnBeginInvocation(componentModel);
				await task;
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

		private async Task<T> HandleAsyncWithResult<T>(Task<T> task, ComponentModel componentModel)
		{
			try
			{
				OnBeginInvocation(componentModel);
				return await task;
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

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncWithResultMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Instance | BindingFlags.NonPublic);

		private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericMethodLookup.GetOrAdd(resultType, _handleAsyncWithResultMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(this, new[] { invocation.ReturnValue });
		}
	}
}
