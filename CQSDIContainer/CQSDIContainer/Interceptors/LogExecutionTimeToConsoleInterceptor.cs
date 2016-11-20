using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public class LogExecutionTimeToConsoleInterceptor : CQSInterceptor
	{
		protected override void InterceptSync(IInvocation invocation)
		{
			var begin = DateTime.UtcNow;
			invocation.Proceed();
			LogExecutionTime(invocation, begin);
		}

		protected override void InterceptAsync(IInvocation invocation, AsynchronousMethodType methodType)
		{
			var begin = DateTime.UtcNow;
			invocation.Proceed();
			switch (methodType)
			{
				case AsynchronousMethodType.Action:
					invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, invocation, begin);
					break;

				case AsynchronousMethodType.Function:
					ExecuteHandleAsyncWithResultUsingReflection(invocation, begin);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Invalid type!!");
			}
		}

		private static async Task HandleAsync(Task task, IInvocation invocation, DateTime begin)
		{
			await task;
			LogExecutionTime(invocation, begin);
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task, IInvocation invocation, DateTime begin)
		{
			var result = await task;
			LogExecutionTime(invocation, begin);
			return result;
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncMethodInfo = typeof(LogExecutionTimeToConsoleInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);
		
		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation, DateTime begin)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericMethodLookup.GetOrAdd(resultType, _handleAsyncMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(null, new[] { invocation.ReturnValue, invocation, begin });
		}

		private static void LogExecutionTime(IInvocation invocation, DateTime begin)
		{
			var end = DateTime.UtcNow;
			Console.WriteLine($"{invocation.Method.DeclaringType} measured time: {(end - begin).TotalMilliseconds} ms");
		}
	}
}
