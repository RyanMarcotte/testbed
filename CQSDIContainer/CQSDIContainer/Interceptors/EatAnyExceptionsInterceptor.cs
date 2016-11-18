﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public class EatAnyExceptionsInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
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
	}
}
