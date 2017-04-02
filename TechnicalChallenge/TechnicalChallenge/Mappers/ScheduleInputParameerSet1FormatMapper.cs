using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Constants;
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
			var searchStart = source.StartDate.AddMilliseconds(1);
			var weeksInMonth = _weeksInMonthCache.GetOrAdd(new MonthAndYear(searchStart.Month, searchStart.Year), monthAndYear =>
			{
				var result = new Dictionary<WeekSchedule, IEnumerable<DayAndDayOfWeek>>();
				var dayAndDayOfWeekCollection = new List<DayAndDayOfWeek>();
				var currentWeek = WeekSchedule.FirstWeek;
				var currentDay = new DateTime(monthAndYear.Year, monthAndYear.Month, 1);

				// keep processing until we're done with all days in the month
				// do not process WeekSchedule.LastWeek, as that is handled differently
				while ((currentDay.Month == monthAndYear.Month) && (currentWeek != WeekSchedule.LastWeek))
				{
					dayAndDayOfWeekCollection.Add(currentDay.ToDayAndDayOfWeek());
					
					// next day is going to start a new week, so cache the dayAndDayOfWeekCollection
					if (currentDay.DayOfWeek == DayOfWeek.Saturday)
					{
						result.Add(currentWeek, dayAndDayOfWeekCollection.ToArray());
						dayAndDayOfWeekCollection.Clear();
						currentWeek++;
					}
					currentDay = currentDay.AddDays(1);
				}

				dayAndDayOfWeekCollection.Clear();
				var lastDayOfMonth = new DateTime(monthAndYear.Year, monthAndYear.Month, 1).AddMonths(1) - TimeSpan.FromDays(1);
				currentDay = new DateTime(monthAndYear.Year, monthAndYear.Month, 1).AddMonths(1) - TimeSpan.FromDays(1);
				while ((currentDay.DayOfWeek != DayOfWeek.Saturday) || (currentDay == lastDayOfMonth))
				{
					dayAndDayOfWeekCollection.Add(currentDay.ToDayAndDayOfWeek());
					currentDay = currentDay - TimeSpan.FromDays(1);
				}
				
				result.Add(WeekSchedule.LastWeek, dayAndDayOfWeekCollection.Select(x => x).Reverse().ToArray());
				return result;
			});
			var weekOfMonthThatSearchFallsIn = source.WeekOfMonth;


			return null;
		}

		private class MonthAndYear : IEquatable<MonthAndYear>
		{
			public MonthAndYear(int month, int year)
			{
				Month = month;
				Year = year;
			}

			public int Month { get; }
			public int Year { get; }

			public bool Equals(MonthAndYear other)
			{
				return (other != null) && (Month == other.Month) && (Year == other.Year);
			}
		}
	}

	public class DayAndDayOfWeek
	{
		public DayAndDayOfWeek(int day, DayOfWeek dayOfWeek)
		{
			Day = day;
			DayOfWeek = dayOfWeek;
		}

		public int Day { get; }
		public DayOfWeek DayOfWeek { get; }
	}

	public static class DateTimeExtensions
	{
		public static DayAndDayOfWeek ToDayAndDayOfWeek(this DateTime dateTime)
		{
			return new DayAndDayOfWeek(dateTime.Day, dateTime.DayOfWeek);
		}
	}
}
