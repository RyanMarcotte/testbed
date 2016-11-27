using System;
using System.Collections.Concurrent;
using System.Reflection;
using Castle.Core;
using IQ.CQS.Attributes;
using IQ.CQS.Interceptors.Exceptions;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;

namespace IQ.CQS.Interceptors
{
	internal class LogPerformanceMetricsInterceptor : CQSInterceptorWithExceptionHandling
	{
		private static readonly ConcurrentDictionary<InvocationInstance, DateTime> _startTimeLookup = new ConcurrentDictionary<InvocationInstance, DateTime>();

		public LogPerformanceMetricsInterceptor(ILogPerformanceMetricsForCQSHandlers executionTimeLogger)
		{
			if (executionTimeLogger == null)
				throw new ArgumentNullException(nameof(executionTimeLogger));

			ExecutionTimeLogger = executionTimeLogger;
		}

		public ILogPerformanceMetricsForCQSHandlers ExecutionTimeLogger { get; }

		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			_startTimeLookup.TryAdd(invocationInstance, DateTime.UtcNow);
		}
		
		protected override void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			var end = DateTime.UtcNow;
			var threshold = TimeSpan.FromMilliseconds(componentModel.Implementation.GetCustomAttribute<LogExecutionTimeAttribute>()?.ThresholdInMilliseconds ?? LogExecutionTimeAttribute.MaximumThreshold);

			DateTime begin;
			if (!_startTimeLookup.TryGetValue(invocationInstance, out begin))
				throw new TransactionScopeNotFoundForInvocationException(invocationInstance);

			ExecutionTimeLogger.LogPerformanceMetrics(componentModel.Implementation, invocationInstance.ParameterObject, end - begin, threshold);
			_startTimeLookup.TryRemove(invocationInstance, out begin);
		}
	}
}
