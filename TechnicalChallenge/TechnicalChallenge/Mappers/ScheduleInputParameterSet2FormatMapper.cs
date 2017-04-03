using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Models;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Mappers
{
	/// <summary>
	/// Maps a schedule input set to the next execution time after the start date+time.
	/// </summary>
	public class ScheduleInputParameterSet2FormatMapper : IMapper<ScheduleInputParameterSet2Format, DateTime?>
	{
		public DateTime? Map(ScheduleInputParameterSet2Format source)
		{
			// first search within the month that the startDate belongs to (just in case the next execution is later that month)
			var searchStart = source.PreviousExecutionTime?.AddMilliseconds(1) ?? source.StartDate;
			var monthAndYear = searchStart.ToMonthAndYear();
			var nextDayToExecuteInCurrentMonthAndYear = fdsjkfldjlsk(source, monthAndYear, searchStart);
			if (nextDayToExecuteInCurrentMonthAndYear != null)
				return nextDayToExecuteInCurrentMonthAndYear;

			// next execution is not later in the week, so look at next month
			var nextScheduledMonthAndYear = source.GetNextScheduledMonthAndYear(searchStart);
			var nextDayToExecuteInFutureMonthAndYear = fdsjkfldjlsk(source, nextScheduledMonthAndYear, searchStart);
			return nextDayToExecuteInFutureMonthAndYear;
		}

		private static DateTime? fdsjkfldjlsk(ScheduleInputParameterSet2Format source, MonthAndYear monthAndYear, DateTime searchStart)
		{
			var daysToExecuteInCurrentMonthAndYear = source.DaysScheduled.Select(x => monthAndYear.ToDateTime(x, source.ExecutionStartTime));
			var nextDaysToExecuteInCurrentMonthAndYear = daysToExecuteInCurrentMonthAndYear.Where(x => x > searchStart).ToArray();
			if (nextDaysToExecuteInCurrentMonthAndYear.Any())
				return nextDaysToExecuteInCurrentMonthAndYear.First();

			return null;
		}
	}
}