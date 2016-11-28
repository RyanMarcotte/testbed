using System;
using System.Collections.Concurrent;
using System.Reflection;
using Castle.Core;
using IQ.CQS.Attributes;
using IQ.CQS.Interceptors.Exceptions;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;

namespace IQ.CQS.Interceptors
{
	/// <summary>
	/// Interceptor for CQS handlers.  Logs performance metrics.
	/// </summary>
	public class LogPerformanceMetricsInterceptor : CQSInterceptorWithExceptionHandling
	{
		private static readonly ConcurrentDictionary<InvocationInstance, DateTime> _startTimeLookup = new ConcurrentDictionary<InvocationInstance, DateTime>();

		/// <summary>
		/// Initializes a new instance of the <see cref="LogPerformanceMetricsInterceptor"/> class.
		/// </summary>
		/// <param name="performanceMetricsLogger">The performance metrics logger.</param>
		public LogPerformanceMetricsInterceptor(ILogPerformanceMetricsForCQSHandlers performanceMetricsLogger)
		{
			if (performanceMetricsLogger == null)
				throw new ArgumentNullException(nameof(performanceMetricsLogger));

			PerformanceMetricsLogger = performanceMetricsLogger;
		}

		/// <summary>
		/// Gets the performance metrics logger.
		/// </summary>
		public ILogPerformanceMetricsForCQSHandlers PerformanceMetricsLogger { get; }

		/// <summary>
		/// Called just before beginning handler invocation.  Use for setup.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			_startTimeLookup.TryAdd(invocationInstance, DateTime.UtcNow);
		}

		/// <summary>
		/// Always called just before returning control to the caller.  Use for teardown.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			var end = DateTime.UtcNow;
			var threshold = TimeSpan.FromMilliseconds(componentModel.Implementation.GetCustomAttribute<LogExecutionTimeAttribute>()?.ThresholdInMilliseconds ?? LogExecutionTimeAttribute.MaximumThreshold);

			DateTime begin;
			if (!_startTimeLookup.TryGetValue(invocationInstance, out begin))
				throw new TransactionScopeNotFoundForInvocationException(invocationInstance);

			PerformanceMetricsLogger.LogPerformanceMetrics(componentModel.Implementation, invocationInstance.ParameterObject, end - begin, threshold);
			_startTimeLookup.TryRemove(invocationInstance, out begin);
		}
	}
}
