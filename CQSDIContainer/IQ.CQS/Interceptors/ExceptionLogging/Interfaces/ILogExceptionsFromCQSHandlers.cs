using System;

namespace IQ.CQS.Interceptors.ExceptionLogging.Interfaces
{
	public interface ILogExceptionsFromCQSHandlers
	{
		void LogException(Exception ex);
	}
}