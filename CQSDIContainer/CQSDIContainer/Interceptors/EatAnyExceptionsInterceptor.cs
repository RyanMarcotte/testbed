using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public class EatAnyExceptionsInterceptor : CQSInterceptor
	{
		protected override void InterceptSync(IInvocation invocation)
		{
			try
			{
				invocation.Proceed();
			}
			catch
			{
				Console.WriteLine("We ate the exception...");
			}
		}

		protected override void InterceptAsync(IInvocation invocation)
		{
			throw new NotImplementedException();
		}
	}
}
