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
			var searchStart = source.StartDate.AddMilliseconds(1);
			var monthAndYear = searchStart.ToMonthAndYear();
			var weeksInMonth = _weeksInMonthCache.GetOrAdd(monthAndYear, key => key.GetWeeksOfMonth());
			var weekOfMonthThatSearchFallsIn = weeksInMonth[source.WeekOfMonth];
			foreach (var dayAndDayOfWeek in weekOfMonthThatSearchFallsIn.Where(x => MakeDateTime(monthAndYear, x.Day, source.ExecutionStartTime) > searchStart))
			{
				if (MatchInSchedule(source, dayAndDayOfWeek))
					return MakeDateTime(monthAndYear, dayAndDayOfWeek.Day, source.ExecutionStartTime);
			}

			// next execution is not later in the week, so look at next month
			var nextScheduledMonthAndYear = GetNextScheduledMonth(source, searchStart);
			var weeksInNextMonth = _weeksInMonthCache.GetOrAdd(nextScheduledMonthAndYear, key => key.GetWeeksOfMonth());
			var weekOfNextMonthThatSearchFallsIn = weeksInNextMonth[source.WeekOfMonth];
			foreach (var dayAndDayOfWeek in weekOfNextMonthThatSearchFallsIn)
			{
				if (MatchInSchedule(source, dayAndDayOfWeek))
					return MakeDateTime(nextScheduledMonthAndYear, dayAndDayOfWeek.Day, source.ExecutionStartTime);
			}

			return null;
		}

		private static DateTime MakeDateTime(MonthAndYear monthAndYear, int day, TimeSpan timeOfDay)
		{
			return new DateTime(monthAndYear.Year, monthAndYear.Month, day, timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds);
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

		private static MonthAndYear GetNextScheduledMonth(ScheduleInputParameterSet1Format source, DateTime dateTime)
		{
			var nextMonth = dateTime.AddMonths(1);
			while (!(source.ScheduledForJanuary && (nextMonth.Month == 1))
					&& !(source.ScheduledForFebruary && (nextMonth.Month == 2))
					&& !(source.ScheduledForMarch && (nextMonth.Month == 3))
					&& !(source.ScheduledForApril && (nextMonth.Month == 4))
					&& !(source.ScheduledForMay && (nextMonth.Month == 5))
					&& !(source.ScheduledForJune && (nextMonth.Month == 6))
					&& !(source.ScheduledForJuly && (nextMonth.Month == 7))
					&& !(source.ScheduledForAugust && (nextMonth.Month == 8))
					&& !(source.ScheduledForSeptember && (nextMonth.Month == 9))
					&& !(source.ScheduledForOctober && (nextMonth.Month == 10))
					&& !(source.ScheduledForNovember && (nextMonth.Month == 11))
					&& !(source.ScheduledForDecember && (nextMonth.Month == 12)))
			{
				nextMonth = nextMonth.AddMonths(1);
			}

			var nextMonthAndYear = nextMonth.ToMonthAndYear();
			return nextMonthAndYear;
		}
	}
}
