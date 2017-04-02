using System;

namespace TechnicalChallenge.Parameters.Interfaces
{
	public interface IScheduleParameters<out TParameter>
	{
		DateTime StartDate { get; }
		DateTime? StopDate { get; }
		TParameter WithNewStartDate(DateTime result);
	}
}