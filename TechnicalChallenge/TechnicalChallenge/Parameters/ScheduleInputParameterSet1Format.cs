using System;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Mappers;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Parameters
{
	public class ScheduleInputParameterSet1Format : IScheduleParameters<ScheduleInputParameterSet1Format>, IHoldInformationAboutWhichMonthsAreScheduled
	{
		public ScheduleInputParameterSet1Format(
			MonthSchedule monthSchedule,
			string weekOfMonth,
			DaySchedule daySchedule,
			TimeSpan executionStartTime,
			DateTime startDate,
			DateTime? stopDate)
		{
			if (monthSchedule == MonthSchedule.None)
				throw new InvalidOperationException("Must schedule in at least one month!");
			if (daySchedule == DaySchedule.None)
				throw new InvalidOperationException("Must schedule on at least one day!");

			PreviousExecutionTime = null;

			ScheduledForJanuary = monthSchedule.HasFlag(MonthSchedule.January);
			ScheduledForFebruary = monthSchedule.HasFlag(MonthSchedule.February);
			ScheduledForMarch = monthSchedule.HasFlag(MonthSchedule.March);
			ScheduledForApril = monthSchedule.HasFlag(MonthSchedule.April);
			ScheduledForMay = monthSchedule.HasFlag(MonthSchedule.May);
			ScheduledForJune = monthSchedule.HasFlag(MonthSchedule.June);
			ScheduledForJuly = monthSchedule.HasFlag(MonthSchedule.July);
			ScheduledForAugust = monthSchedule.HasFlag(MonthSchedule.August);
			ScheduledForSeptember = monthSchedule.HasFlag(MonthSchedule.September);
			ScheduledForOctober = monthSchedule.HasFlag(MonthSchedule.October);
			ScheduledForNovember = monthSchedule.HasFlag(MonthSchedule.November);
			ScheduledForDecember = monthSchedule.HasFlag(MonthSchedule.December);

			WeekOfMonth = new StringToWeekScheduleMapper().Map(weekOfMonth);

			ScheduledForSunday = daySchedule.HasFlag(DaySchedule.Sunday);
			ScheduledForMonday = daySchedule.HasFlag(DaySchedule.Monday);
			ScheduledForTuesday = daySchedule.HasFlag(DaySchedule.Tuesday);
			ScheduledForWednesday = daySchedule.HasFlag(DaySchedule.Wednesday);
			ScheduledForThursday = daySchedule.HasFlag(DaySchedule.Thursday);
			ScheduledForFriday = daySchedule.HasFlag(DaySchedule.Friday);
			ScheduledForSaturday = daySchedule.HasFlag(DaySchedule.Saturday);

			ExecutionStartTime = executionStartTime;
			StartDate = startDate;
			StopDate = stopDate;
		}

		private ScheduleInputParameterSet1Format(ScheduleInputParameterSet1Format existingParameters, DateTime newPreviousExecutionTime)
		{
			PreviousExecutionTime = newPreviousExecutionTime;

			ScheduledForJanuary = existingParameters.ScheduledForJanuary;
			ScheduledForFebruary = existingParameters.ScheduledForFebruary;
			ScheduledForMarch = existingParameters.ScheduledForMarch;
			ScheduledForApril = existingParameters.ScheduledForApril;
			ScheduledForMay = existingParameters.ScheduledForMay;
			ScheduledForJune = existingParameters.ScheduledForJune;
			ScheduledForJuly = existingParameters.ScheduledForJuly;
			ScheduledForAugust = existingParameters.ScheduledForAugust;
			ScheduledForSeptember = existingParameters.ScheduledForSeptember;
			ScheduledForOctober = existingParameters.ScheduledForOctober;
			ScheduledForNovember = existingParameters.ScheduledForNovember;
			ScheduledForDecember = existingParameters.ScheduledForDecember;

			WeekOfMonth = existingParameters.WeekOfMonth;

			ScheduledForSunday = existingParameters.ScheduledForSunday;
			ScheduledForMonday = existingParameters.ScheduledForMonday;
			ScheduledForTuesday = existingParameters.ScheduledForTuesday;
			ScheduledForWednesday = existingParameters.ScheduledForWednesday;
			ScheduledForThursday = existingParameters.ScheduledForThursday;
			ScheduledForFriday = existingParameters.ScheduledForFriday;
			ScheduledForSaturday = existingParameters.ScheduledForSaturday;

			ExecutionStartTime = existingParameters.ExecutionStartTime;
			StartDate = existingParameters.StartDate;
			StopDate = existingParameters.StopDate;
		}

		public DateTime? PreviousExecutionTime { get; }

		public bool ScheduledForJanuary { get; }
		public bool ScheduledForFebruary { get; }
		public bool ScheduledForMarch { get; }
		public bool ScheduledForApril { get; }
		public bool ScheduledForMay { get; }
		public bool ScheduledForJune { get; }
		public bool ScheduledForJuly { get; }
		public bool ScheduledForAugust { get; }
		public bool ScheduledForSeptember { get; }
		public bool ScheduledForOctober { get; }
		public bool ScheduledForNovember { get; }
		public bool ScheduledForDecember { get; }

		public WeekSchedule WeekOfMonth { get; }

		public bool ScheduledForSunday { get; }
		public bool ScheduledForMonday { get; }
		public bool ScheduledForTuesday { get; }
		public bool ScheduledForWednesday { get; }
		public bool ScheduledForThursday { get; }
		public bool ScheduledForFriday { get; }
		public bool ScheduledForSaturday { get; }

		public TimeSpan ExecutionStartTime { get; }
		public DateTime StartDate { get; }
		public DateTime? StopDate { get; }

		public ScheduleInputParameterSet1Format WithNewPreviousExecutionTime(DateTime executionTime)
		{
			return new ScheduleInputParameterSet1Format(this, executionTime);
		}
	}
}
