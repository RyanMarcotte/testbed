using System;

namespace TechnicalChallenge.Parameters.Interfaces
{
	public interface IScheduleParameters<out TParameter>
	{
		DateTime? PreviousExecutionTime { get; }

		DateTime StartDate { get; }
		DateTime? StopDate { get; }

		TParameter WithNewPreviousExecutionTime(DateTime executionTime);
	}
}