using System;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Parameters
{
	public class ScheduleParametersAndPreviousExecutionTime<TSchedule>
		where TSchedule : IHoldInformationAboutScheduleInterval
	{
		public ScheduleParametersAndPreviousExecutionTime(TSchedule schedule, DateTime previousExecutionTime)
		{
			Schedule = schedule;
			PreviousExecutionTime = previousExecutionTime;
		}

		public TSchedule Schedule { get; }
		public DateTime PreviousExecutionTime { get; }
	}
}