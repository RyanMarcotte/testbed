using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Models;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Mappers
{
	public class ScheduleInputParameterSet1FormatMapper : IMapper<ScheduleParametersAndPreviousExecutionTime<ScheduleInputParameterSet1Format>, DateTime?>
	{
		private static readonly ConcurrentDictionary<MonthAndYear, IReadOnlyDictionary<WeekSchedule, IEnumerable<DayAndDayOfWeek>>> _weeksInMonthCache = new ConcurrentDictionary<MonthAndYear, IReadOnlyDictionary<WeekSchedule, IEnumerable<DayAndDayOfWeek>>>();

		public DateTime? Map(ScheduleParametersAndPreviousExecutionTime<ScheduleInputParameterSet1Format> source)
		{
			// first search within the week that the startDate belongs to (just in case the next execution is later that week)
			var monthAndYear = source.PreviousExecutionTime.ToMonthAndYear();
			var nextDayToExecuteInSameWeek = GetNextExecutionTimeInMonthAndYear(source, monthAndYear);
			if (nextDayToExecuteInSameWeek != null)
				return nextDayToExecuteInSameWeek;
			
			// next execution is not later in the same week, so look in the next scheduled month
			var nextScheduledMonthAndYear = source.Schedule.GetNextScheduledMonthAndYear(source.PreviousExecutionTime);
			var nextDayToExecuteInNextScheduledMonth = GetNextExecutionTimeInMonthAndYear(source, nextScheduledMonthAndYear);
			return nextDayToExecuteInNextScheduledMonth;
		}

		private static DateTime? GetNextExecutionTimeInMonthAndYear(ScheduleParametersAndPreviousExecutionTime<ScheduleInputParameterSet1Format> source, MonthAndYear monthAndYear)
		{
			var weeksInMonth = _weeksInMonthCache.GetOrAdd(monthAndYear, key => key.GetWeeksOfMonth());
			var weekOfMonthThatSearchFallsIn = weeksInMonth[source.Schedule.WeekOfMonth];
			foreach (var dayAndDayOfWeek in weekOfMonthThatSearchFallsIn.Where(x => monthAndYear.ToDateTime(x.Day, source.Schedule.ExecutionStartTime) > source.PreviousExecutionTime))
			{
				if (MatchInSchedule(source.Schedule, dayAndDayOfWeek))
					return monthAndYear.ToDateTime(dayAndDayOfWeek.Day, source.Schedule.ExecutionStartTime);
			}

			return null;
		}

		private static bool MatchInSchedule(ScheduleInputParameterSet1Format schedule, DayAndDayOfWeek dayAndDayOfWeek)
		{
			return (schedule.ScheduledForSunday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Sunday))
					|| (schedule.ScheduledForMonday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Monday))
					|| (schedule.ScheduledForTuesday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Tuesday))
					|| (schedule.ScheduledForWednesday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Wednesday))
					|| (schedule.ScheduledForThursday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Thursday))
					|| (schedule.ScheduledForFriday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Friday))
					|| (schedule.ScheduledForSaturday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Saturday));
		}
	}
}
