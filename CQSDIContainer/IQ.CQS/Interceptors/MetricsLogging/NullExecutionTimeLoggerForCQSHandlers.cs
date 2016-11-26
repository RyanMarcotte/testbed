using System;
using IQ.CQS.Interceptors.MetricsLogging.Interfaces;

namespace IQ.CQS.Interceptors.MetricsLogging
{
	internal class NullExecutionTimeLoggerForCQSHandlers : ILogExecutionTimeOfCQSHandlers
	{
		public void LogExecutionTime(Type handlerType, TimeSpan executionTime, TimeSpan threshold)
		{
			Console.WriteLine($"[{handlerType}] measured time: {executionTime.TotalMilliseconds} ms");
			if (executionTime >= threshold)
				Console.WriteLine($"OVER THRESHOLD!! ({threshold} ms)");
		}
	}
}
