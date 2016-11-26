using System;
using IQ.CQS.Interceptors.MetricsLogging.Interfaces;

namespace IQ.CQS.Interceptors.MetricsLogging
{
	public class ExecutionTimeLoggerForCQSHandlers : ILogExecutionTimeOfCQSHandlers
	{
		public void LogExecutionTime(Type handlerType, TimeSpan executionTime, TimeSpan threshold)
		{
			Console.WriteLine($"[{handlerType}] measured time: {executionTime.TotalMilliseconds} ms");
			if (executionTime >= threshold)
				Console.WriteLine($"OVER THRESHOLD!! ({threshold} ms)");
		}
	}
}
