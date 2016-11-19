using System;
using System.Collections.Generic;
using System.Linq;
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
			var end = DateTime.UtcNow;
			Console.WriteLine($"{invocation.Method.DeclaringType} measured time: {(end - begin).TotalMilliseconds} ms");
		}

		protected override void InterceptAsync(IInvocation invocation)
		{
			throw new NotImplementedException();
		}
	}
}
