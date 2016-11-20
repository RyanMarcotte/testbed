using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;

namespace CQSDIContainer.Interceptors
{
	public class LogExecutionTimeToConsoleInterceptor : CQSInterceptor
	{
		private readonly ILogExecutionTimeOfCQSHandlers _executionTimeLogger;

		public LogExecutionTimeToConsoleInterceptor(ILogExecutionTimeOfCQSHandlers executionTimeLogger)
		{
			if (executionTimeLogger == null)
				throw new ArgumentNullException(nameof(executionTimeLogger));

			_executionTimeLogger = executionTimeLogger;
		}

		protected override bool ApplyToNestedHandlers => false;

		protected override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			var begin = DateTime.UtcNow;
			invocation.Proceed();
			LogExecutionTime(componentModel.Implementation, _executionTimeLogger, begin);
		}

		protected override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
		{
			var handlerType = componentModel.Implementation;
			var begin = DateTime.UtcNow;
			invocation.Proceed();
			switch (methodType)
			{
				case AsynchronousMethodType.Action:
					invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, handlerType, _executionTimeLogger, begin);
					break;

				case AsynchronousMethodType.Function:
					ExecuteHandleAsyncWithResultUsingReflection(invocation, handlerType, _executionTimeLogger, begin);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Invalid type!!");
			}
		}

		private static async Task HandleAsync(Task task, Type handlerType, ILogExecutionTimeOfCQSHandlers executionTimeLogger, DateTime begin)
		{
			await task;
			LogExecutionTime(handlerType, executionTimeLogger, begin);
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task, Type handlerType, ILogExecutionTimeOfCQSHandlers executionTimeLogger, DateTime begin)
		{
			var result = await task;
			LogExecutionTime(handlerType, executionTimeLogger, begin);
			return result;
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncMethodInfo = typeof(LogExecutionTimeToConsoleInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation, Type handlerType, ILogExecutionTimeOfCQSHandlers executionTimeLogger, DateTime begin)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericMethodLookup.GetOrAdd(resultType, _handleAsyncMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(null, new[] { invocation.ReturnValue, handlerType, executionTimeLogger, begin });
		}

		private static void LogExecutionTime(Type handlerType, ILogExecutionTimeOfCQSHandlers executionTimeLogger, DateTime begin)
		{
			var end = DateTime.UtcNow;
			executionTimeLogger.LogExecutionTime(handlerType, end - begin);
		}
	}
}
