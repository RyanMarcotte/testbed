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
	/// <summary>
	/// Maps a schedule input set to the next execution time after the start date+time.
	/// </summary>
	public class ScheduleInputParameterSet1FormatMapper : IMapper<ScheduleInputParameterSet1Format, DateTime?>
	{
		private static readonly ConcurrentDictionary<MonthAndYear, IReadOnlyDictionary<WeekSchedule, IEnumerable<DayAndDayOfWeek>>> _weeksInMonthCache = new ConcurrentDictionary<MonthAndYear, IReadOnlyDictionary<WeekSchedule, IEnumerable<DayAndDayOfWeek>>>();

		public DateTime? Map(ScheduleInputParameterSet1Format source)
		{
			// first search within the week that the startDate belongs to (just in case the next execution is later that week)
			var searchStart = source.PreviousExecutionTime?.AddMilliseconds(1) ?? source.StartDate;
			var monthAndYear = searchStart.ToMonthAndYear();
			var weeksInMonth = _weeksInMonthCache.GetOrAdd(monthAndYear, key => key.GetWeeksOfMonth());
			var weekOfMonthThatSearchFallsIn = weeksInMonth[source.WeekOfMonth];
			foreach (var dayAndDayOfWeek in weekOfMonthThatSearchFallsIn.Where(x => monthAndYear.ToDateTime(x.Day, source.ExecutionStartTime) > searchStart))
			{
				if (MatchInSchedule(source, dayAndDayOfWeek))
					return monthAndYear.ToDateTime(dayAndDayOfWeek.Day, source.ExecutionStartTime);
			}

			// next execution is not later in the week, so look in the next scheduled month
			var nextScheduledMonthAndYear = source.GetNextScheduledMonthAndYear(searchStart);
			var weeksInNextMonth = _weeksInMonthCache.GetOrAdd(nextScheduledMonthAndYear, key => key.GetWeeksOfMonth());
			var weekOfNextMonthThatSearchFallsIn = weeksInNextMonth[source.WeekOfMonth];
			foreach (var dayAndDayOfWeek in weekOfNextMonthThatSearchFallsIn)
			{
				if (MatchInSchedule(source, dayAndDayOfWeek))
					return nextScheduledMonthAndYear.ToDateTime(dayAndDayOfWeek.Day, source.ExecutionStartTime);
			}

			return null;
		}

		private static bool MatchInSchedule(ScheduleInputParameterSet1Format source, DayAndDayOfWeek dayAndDayOfWeek)
		{
			return (source.ScheduledForSunday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Sunday))
					|| (source.ScheduledForMonday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Monday))
					|| (source.ScheduledForTuesday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Tuesday))
					|| (source.ScheduledForWednesday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Wednesday))
					|| (source.ScheduledForThursday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Thursday))
					|| (source.ScheduledForFriday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Friday))
					|| (source.ScheduledForSaturday && (dayAndDayOfWeek.DayOfWeek == DayOfWeek.Saturday));
		}
	}
}
