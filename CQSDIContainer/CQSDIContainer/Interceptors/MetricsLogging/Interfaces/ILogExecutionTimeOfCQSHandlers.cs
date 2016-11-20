using System;

namespace CQSDIContainer.Interceptors.MetricsLogging.Interfaces
{
	public interface ILogExecutionTimeOfCQSHandlers
	{
		void LogExecutionTime(Type handlerType, TimeSpan executionTime, TimeSpan threshold);
	}
}