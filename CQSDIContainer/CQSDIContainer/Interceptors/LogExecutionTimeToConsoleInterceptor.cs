using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public class LogExecutionTimeToConsoleInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			var begin = DateTime.UtcNow;
			invocation.Proceed();
			var end = DateTime.UtcNow;
			Console.WriteLine($"{invocation.Method.DeclaringType} measured time: {(end - begin).TotalMilliseconds} ms");
		}
	}
}
