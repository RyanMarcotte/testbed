using System;

namespace TechnicalChallenge.Models
{
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
}