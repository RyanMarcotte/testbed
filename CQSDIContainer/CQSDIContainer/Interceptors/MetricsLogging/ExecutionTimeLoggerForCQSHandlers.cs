using System;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;

namespace CQSDIContainer.Interceptors.MetricsLogging
{
	public class ExecutionTimeLoggerForCQSHandlers : ILogExecutionTimeOfCQSHandlers
	{
		public void LogExecutionTime(Type handlerType, TimeSpan executionTime)
		{
			Console.WriteLine($"[{handlerType}] measured time: {executionTime.TotalMilliseconds} ms");
		}
	}
}
