using System;
using System.Collections.Generic;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Mappers;

namespace TechnicalChallenge.Parameters
{
	public class ScheduleInputParameterSet2Format
	{
		public ScheduleInputParameterSet2Format(
			MonthSchedule monthSchedule,
			string commaDelimitedStringOfDaysScheduled,
			TimeSpan executionStartTime,
			DateTime startDate,
			DateTime? stopDate)
		{
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
			DaysScheduled = new CommaDelimitedStringToEnumerableCollectionOfIntegersMapper().Map(commaDelimitedStringOfDaysScheduled);
			ExecutionStartTime = executionStartTime;
			StartDate = startDate;
			StopDate = stopDate;
		}

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

		public IEnumerable<int> DaysScheduled { get; }

		public TimeSpan ExecutionStartTime { get; }
		public DateTime StartDate { get; }
		public DateTime? StopDate { get; }
	}
}