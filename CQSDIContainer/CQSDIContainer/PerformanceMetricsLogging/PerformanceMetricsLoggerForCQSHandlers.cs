using System;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;

namespace IQ.CQS.Lab.PerformanceMetricsLogging
{
	public class PerformanceMetricsLoggerForCQSHandlers : ILogPerformanceMetricsForCQSHandlers
	{
		public void LogPerformanceMetrics(Type handlerType, TimeSpan executionTime, TimeSpan threshold)
		{
			Console.WriteLine($"[{handlerType}] measured time: {executionTime.TotalMilliseconds} ms");
			if (executionTime >= threshold)
				Console.WriteLine($"OVER THRESHOLD!! ({threshold} ms)");
		}
	}
}
