using System;
using Ploeh.AutoFixture;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Parameters;

namespace TechnicalChallenge.Tests.Customizations
{
	public class ScheduleInputParameterSet2Customization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register(() =>
			{
				const MonthSchedule MONTH_SCHEDULE = MonthSchedule.January | MonthSchedule.March | MonthSchedule.June | MonthSchedule.September;
				const string DAY_SCHEDULE = "1,7,14,21,28";

				var startTime = TimeSpan.FromHours(9);          // 09:00:00.000
				var startDate = new DateTime(2017, 1, 1);       // 2017-01-01 00:00:00.000
				return new ScheduleInputParameterSet2Format(MONTH_SCHEDULE, DAY_SCHEDULE, startTime, startDate, null);
			});
		}
	}
}