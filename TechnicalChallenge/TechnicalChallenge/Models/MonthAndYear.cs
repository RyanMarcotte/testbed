using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Mappers;

namespace TechnicalChallenge.Models
{
	public class MonthAndYear : IEquatable<MonthAndYear>
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

		public override int GetHashCode()
		{
			unchecked
			{
				return (17 * Month) + (23 * Year);
			}
		}
	}

	public static class MonthAndYearExtensions
	{
		public static DateTime ToDateTime(this MonthAndYear monthAndYear)
		{
			return new DateTime(monthAndYear.Year, monthAndYear.Month, 1);
		}

		public static DateTime ToDateTime(this MonthAndYear monthAndYear, int day, TimeSpan timeOfDay)
		{
			return new DateTime(monthAndYear.Year, monthAndYear.Month, day, timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds);
		}

		public static IReadOnlyDictionary<WeekSchedule, IEnumerable<DayAndDayOfWeek>> GetWeeksOfMonth(this MonthAndYear monthAndYear)
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
		}
	}
}
