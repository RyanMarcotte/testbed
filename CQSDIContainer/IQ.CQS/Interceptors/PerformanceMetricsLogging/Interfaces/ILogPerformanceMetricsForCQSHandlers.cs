using System;

namespace IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces
{
	public interface ILogPerformanceMetricsForCQSHandlers
	{
		void LogPerformanceMetrics(Type handlerType, TimeSpan executionTime, TimeSpan threshold);
	}
}