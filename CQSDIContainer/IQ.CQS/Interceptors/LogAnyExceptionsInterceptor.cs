using System;
using Castle.Core;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;

namespace IQ.CQS.Interceptors
{
	/// <summary>
	/// Interceptor for CQS handlers.  Logs exceptions.
	/// </summary>
	public class LogAnyExceptionsInterceptor : CQSInterceptorWithExceptionHandling
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LogAnyExceptionsInterceptor"/> class.
		/// </summary>
		/// <param name="exceptionLogger">The exception logger.</param>
		public LogAnyExceptionsInterceptor(ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			if (exceptionLogger == null)
				throw new ArgumentNullException(nameof(exceptionLogger));

			ExceptionLogger = exceptionLogger;
		}

		/// <summary>
		/// Gets the exception logger.
		/// </summary>
		public ILogExceptionsFromCQSHandlers ExceptionLogger { get; }

		/// <summary>
		/// Called when an exception has been thrown during invocation.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="ex">The exception.</param>
		protected override void OnException(InvocationInstance invocationInstance, ComponentModel componentModel, Exception ex)
		{
			ExceptionLogger.LogException(ex);
		}
	}
}
