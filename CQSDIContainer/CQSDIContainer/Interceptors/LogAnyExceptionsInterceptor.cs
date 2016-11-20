using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;

namespace CQSDIContainer.Interceptors
{
	public class LogAnyExceptionsInterceptor : CQSInterceptorWithExceptionHandling
	{
		private readonly ILogExceptionsFromCQSHandlers _exceptionLogger;

		public LogAnyExceptionsInterceptor(ILogExceptionsFromCQSHandlers exceptionLogger)
		{
			if (exceptionLogger == null)
				throw new ArgumentNullException(nameof(exceptionLogger));

			_exceptionLogger = exceptionLogger;
		}

		protected override bool ApplyToNestedHandlers => false;

		protected override void OnException(ComponentModel componentModel, Exception ex)
		{
			_exceptionLogger.LogException(ex);
		}
	}
}
