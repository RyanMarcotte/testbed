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
	public class LogAnyExceptionsInterceptor : CQSInterceptor
	{
		protected override void InterceptSync(IInvocation invocation)
		{
			try
			{
				invocation.Proceed();
			}
			catch (Exception ex)
			{
				Console.WriteLine("An exception occured!!");
				Console.WriteLine(ex);
				throw;
			}
		}

		protected override void InterceptAsync(IInvocation invocation, AsynchronousMethodType methodType)
		{
			try
			{
				invocation.Proceed();
				switch (methodType)
				{
					case AsynchronousMethodType.Action:
						invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue);
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
				Console.WriteLine("An exception occured!!");
				Console.WriteLine(ex);
				throw;
			}
		}

		private static async Task HandleAsync(Task task)
		{
			try
			{
				await task;
			}
			catch (Exception ex)
			{
				Console.WriteLine("An exception occured!!");
				Console.WriteLine(ex);
				throw;
			}
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task)
		{
			try
			{
				return await task;
			}
			catch (Exception ex)
			{
				Console.WriteLine("An exception occured!!");
				Console.WriteLine(ex);
				throw;
			}
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncMethodInfo = typeof(LogAnyExceptionsInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericMethodLookup.GetOrAdd(resultType, _handleAsyncMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(null, new[] { invocation.ReturnValue });
		}
	}
}
