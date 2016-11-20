using System;

namespace CQSDIContainer.Attributes
{
	/// <summary>
	/// Attribute for tagging command handlers and query handlers that will have their execution time logged.
	/// An optional threshold parameter can be used to perform additional work if execution time is above a certain amount.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class LogExecutionTimeAttribute : Attribute
	{
		public const uint MaximumThreshold = uint.MaxValue;

		public LogExecutionTimeAttribute(uint thresholdInMilliseconds = MaximumThreshold)
		{
			ThresholdInMilliseconds = thresholdInMilliseconds;
		}

		public uint ThresholdInMilliseconds { get; }
	}
}
