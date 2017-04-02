using System;
using System.Collections.Generic;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Mappers;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Parameters
{
	public class ScheduleInputParameterSet2Format : IScheduleParameters<ScheduleInputParameterSet2Format>
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

		private ScheduleInputParameterSet2Format(ScheduleInputParameterSet2Format existingParameters, DateTime newStartDate)
		{
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

			DaysScheduled = existingParameters.DaysScheduled;

			ExecutionStartTime = existingParameters.ExecutionStartTime;
			StartDate = newStartDate;
			StopDate = existingParameters.StopDate;
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
		
		public ScheduleInputParameterSet2Format WithNewStartDate(DateTime newStartDate)
		{
			return new ScheduleInputParameterSet2Format(this, newStartDate);
		}
	}
}