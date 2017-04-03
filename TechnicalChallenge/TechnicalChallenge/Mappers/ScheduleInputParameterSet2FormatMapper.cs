using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Models;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Mappers
{
	public class ScheduleInputParameterSet2FormatMapper : IMapper<ScheduleParametersAndPreviousExecutionTime<ScheduleInputParameterSet2Format>, DateTime?>
	{
		public DateTime? Map(ScheduleParametersAndPreviousExecutionTime<ScheduleInputParameterSet2Format> source)
		{
			// first search within the month that the startDate belongs to (just in case the next execution is later that month)
			var monthAndYear = source.PreviousExecutionTime.ToMonthAndYear();
			var nextDayToExecuteInCurrentMonthAndYear = GetNextDayToExecuteInMonthAndYear(source.Schedule, monthAndYear, source.PreviousExecutionTime);
			if (nextDayToExecuteInCurrentMonthAndYear != null)
				return nextDayToExecuteInCurrentMonthAndYear;

			// next execution is not later in the month, so look at next scheduled month
			var nextScheduledMonthAndYear = source.Schedule.GetNextScheduledMonthAndYear(source.PreviousExecutionTime);
			var nextDayToExecuteInNextScheduledMonthAndYear = GetNextDayToExecuteInMonthAndYear(source.Schedule, nextScheduledMonthAndYear, source.PreviousExecutionTime);
			return nextDayToExecuteInNextScheduledMonthAndYear;
		}

		private static DateTime? GetNextDayToExecuteInMonthAndYear(ScheduleInputParameterSet2Format schedule, MonthAndYear monthAndYear, DateTime searchStart)
		{
			var daysToExecuteInCurrentMonthAndYear = schedule.DaysScheduled.Select(x => monthAndYear.ToDateTime(x, schedule.ExecutionStartTime));
			var nextDaysToExecuteInCurrentMonthAndYear = daysToExecuteInCurrentMonthAndYear.Where(x => x > searchStart).ToArray();
			if (nextDaysToExecuteInCurrentMonthAndYear.Any())
				return nextDaysToExecuteInCurrentMonthAndYear.First();

			return null;
		}
	}
}