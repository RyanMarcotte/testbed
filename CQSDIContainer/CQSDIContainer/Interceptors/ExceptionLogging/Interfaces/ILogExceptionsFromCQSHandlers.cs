using System;

namespace CQSDIContainer.Interceptors.ExceptionLogging.Interfaces
{
	public interface ILogExceptionsFromCQSHandlers
	{
		void LogException(Exception ex);
	}
}