using System;

namespace IQ.CQS.Interceptors.MetricsLogging.Interfaces
{
	public interface ILogExecutionTimeOfCQSHandlers
	{
		void LogExecutionTime(Type handlerType, TimeSpan executionTime, TimeSpan threshold);
	}
}