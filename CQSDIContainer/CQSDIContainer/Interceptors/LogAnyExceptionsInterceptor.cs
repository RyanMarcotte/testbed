using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public class LogAnyExceptionsInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
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
	}
}
