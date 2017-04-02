using System;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Constants;

namespace TechnicalChallenge.Mappers
{
	public class StringToWeekScheduleMapper : IMapper<string, WeekSchedule>
	{
		public WeekSchedule Map(string source)
		{
			switch (source)
			{
				case "1st":
					return WeekSchedule.FirstWeek;
				case "2nd":
					return WeekSchedule.SecondWeek;
				case "3rd":
					return WeekSchedule.ThirdWeek;
				case "4th":
					return WeekSchedule.FourthWeek;
				case "Last":
					return WeekSchedule.LastWeek;
				default:
					throw new ArgumentOutOfRangeException(nameof(source), "Invalid value!  The only valid values are '1st', '2nd', '3rd', '4th', and 'Last'.");
			}
		}
	}
}