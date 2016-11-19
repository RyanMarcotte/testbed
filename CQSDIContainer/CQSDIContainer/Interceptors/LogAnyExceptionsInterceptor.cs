using System;
using System.Collections.Generic;
using System.Linq;
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

		protected override void InterceptAsync(IInvocation invocation)
		{
			throw new NotImplementedException();
		}
	}
}
