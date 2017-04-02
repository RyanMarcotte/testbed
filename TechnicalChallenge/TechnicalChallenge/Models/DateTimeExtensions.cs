using System;

namespace TechnicalChallenge.Models
{
	public static class DateTimeExtensions
	{
		public static MonthAndYear ToMonthAndYear(this DateTime dateTime)
		{
			return new MonthAndYear(dateTime.Month, dateTime.Year);
		}

		public static DayAndDayOfWeek ToDayAndDayOfWeek(this DateTime dateTime)
		{
			return new DayAndDayOfWeek(dateTime.Day, dateTime.DayOfWeek);
		}
	}
}