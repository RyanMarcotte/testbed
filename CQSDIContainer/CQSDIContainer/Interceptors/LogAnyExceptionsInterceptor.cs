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
	public class LogAnyExceptionsInterceptor : CQSInterceptor
	{
		private readonly ILogExceptionsFromCQSHandlers _exceptionLogger;

		public LogAnyExceptionsInterceptor(ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			if (exceptionLogger == null)
				throw new ArgumentNullException(nameof(exceptionLogger));

			_exceptionLogger = exceptionLogger;
		}

		protected override bool ApplyToNestedHandlers => false;

		protected override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			try
			{
				invocation.Proceed();
			}
			catch (Exception ex)
			{
				_exceptionLogger.LogException(ex);
				throw;
			}
		}

		protected override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
		{
			try
			{
				invocation.Proceed();
				switch (methodType)
				{
					case AsynchronousMethodType.Action:
						invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, _exceptionLogger);
						break;

					case AsynchronousMethodType.Function:
						ExecuteHandleAsyncWithResultUsingReflection(invocation, _exceptionLogger);
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Invalid value!!");
				}
			}
			catch (Exception ex)
			{
				_exceptionLogger.LogException(ex);
				throw;
			}
		}

		private static async Task HandleAsync(Task task, ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			try
			{
				await task;
			}
			catch (Exception ex)
			{
				exceptionLogger.LogException(ex);
				throw;
			}
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task, ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			try
			{
				return await task;
			}
			catch (Exception ex)
			{
				exceptionLogger.LogException(ex);
				throw;
			}
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncWithResultMethodInfo = typeof(LogAnyExceptionsInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation, ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericMethodLookup.GetOrAdd(resultType, _handleAsyncWithResultMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(null, new[] { invocation.ReturnValue, exceptionLogger });
		}
	}
}
