using System;
using Castle.Core;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;

namespace IQ.CQS.Interceptors
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
