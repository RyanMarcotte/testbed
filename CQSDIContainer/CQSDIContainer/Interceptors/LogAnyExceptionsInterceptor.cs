using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;

namespace CQSDIContainer.Interceptors
{
	public class LogAnyExceptionsInterceptor : CQSInterceptorWithExceptionHandling
	{
		public LogAnyExceptionsInterceptor(ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			if (exceptionLogger == null)
				throw new ArgumentNullException(nameof(exceptionLogger));

			ExceptionLogger = exceptionLogger;
		}

		public ILogExceptionsFromCQSHandlers ExceptionLogger { get; }

		protected override void OnException(InvocationInstance invocationInstance, ComponentModel componentModel, Exception ex)
		{
			ExceptionLogger.LogException(ex);
		}
	}
}
