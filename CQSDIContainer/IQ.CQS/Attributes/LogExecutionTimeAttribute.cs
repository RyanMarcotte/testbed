using System;

namespace IQ.CQS.Attributes
{
	/// <summary>
	/// Attribute for tagging CQS handlers that will opt in to have their execution time logged.
	/// An optional threshold parameter can be used to perform additional work if execution time is above a certain amount.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class LogExecutionTimeAttribute : Attribute
	{
		/// <summary>
		/// The default maximum threshold (in milliseconds).
		/// </summary>
		public const uint MaximumThreshold = uint.MaxValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="LogExecutionTimeAttribute"/> class.
		/// </summary>
		/// <param name="thresholdInMilliseconds">The threshold (in milliseconds) at which to start performing additional logging for tracking bad performance.</param>
		public LogExecutionTimeAttribute(uint thresholdInMilliseconds = MaximumThreshold)
		{
			ThresholdInMilliseconds = thresholdInMilliseconds;
		}

		/// <summary>
		/// The threshold (in milliseconds) at which to start performing additional logging for tracking bad performance.
		/// </summary>
		public uint ThresholdInMilliseconds { get; }
	}
}
