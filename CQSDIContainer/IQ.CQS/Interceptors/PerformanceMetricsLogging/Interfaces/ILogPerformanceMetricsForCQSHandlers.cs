using System;

namespace IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces
{
	/// <summary>
	/// Interface for an object responsible for logging performance metrics.
	/// </summary>
	public interface ILogPerformanceMetricsForCQSHandlers
	{
		/// <summary>
		/// Log performance metrics associated with an invocation.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <param name="parameters">The invocation parameters.</param>
		/// <param name="executionTime">The execution time.</param>
		/// <param name="threshold">A threshold value for performing additional logging when 'executionTime' exceeds a certain value.</param>
		void LogPerformanceMetrics(Type handlerType, object parameters, TimeSpan executionTime, TimeSpan threshold);
	}
}