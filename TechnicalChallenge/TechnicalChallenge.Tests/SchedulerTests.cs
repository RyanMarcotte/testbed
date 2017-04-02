using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IQ.Vanilla.Mapping;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using SimpleInjector;
using TechnicalChallenge.Constants;
using TechnicalChallenge.Parameters;
using Xunit;

namespace TechnicalChallenge.Tests
{
    public class SchedulerTests
    {
		[Theory]
		[ArrangementForSchedulerActingOnInputSet1]
	    public void ShouldReturnTheExpectedOutputGivenInputSet1(Scheduler<ScheduleInputParameterSet1Format> sut, ScheduleInputParameterSet1Format parameters)
		{
			var previousExecutionTimeUTC = new DateTime(2017, 1, 30, 9, 0, 0);          // 2017-01-30 09:00:00.000
			var expectedNextExecutionTimeUTC = new DateTime(2017, 3, 27, 9, 0, 0);      // 2017-03-27 09:00:00.000
			sut.GetNextExecuteDate(previousExecutionTimeUTC, parameters).Should().Be(expectedNextExecutionTimeUTC);
		}

		[Theory]
		[ArrangementForSchedulerActingOnInputSet2]
	    public void ShouldReturnTheExpectedOutputGivenInputSet2(Scheduler<ScheduleInputParameterSet2Format> sut, ScheduleInputParameterSet2Format parameters)
	    {
			var previousExecutionTimeUTC = new DateTime(2017, 1, 28, 9, 0, 0);          // 2017-01-28 09:00:00.000
			var expectedNextExecutionTimeUTC = new DateTime(2017, 3, 27, 9, 0, 0);      // 2017-03-01 09:00:00.000
			sut.GetNextExecuteDate(previousExecutionTimeUTC, parameters).Should().Be(expectedNextExecutionTimeUTC);
		}
    }

	public class ArrangementForSchedulerActingOnInputSet1 : AutoDataAttribute
	{
		public ArrangementForSchedulerActingOnInputSet1()
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new SchedulerCustomization<ScheduleInputParameterSet1Format>())
				.Customize(new ScheduleInputParameterSet1Customization()))
		{
			
		}	
	}

	public class SchedulerCustomization<TInputParameter> : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var container = new Container();

			var inputParameterMapperContractType = typeof(IMapper<TInputParameter, InternalSchedulerInputParameters>);
			var inputParameterMapperImplementionTypes = from type in typeof(TInputParameter).Assembly.GetTypes() where type.IsClass && type.GetInterfaces().Contains(inputParameterMapperContractType) select type;
			foreach (var implementationType in inputParameterMapperImplementionTypes)
				container.Register(inputParameterMapperContractType, implementationType, Lifestyle.Singleton);

			container.RegisterSingleton(typeof(Scheduler<TInputParameter>), typeof(Scheduler<TInputParameter>));
			container.Verify();

			fixture.Register(() => container.GetInstance<Scheduler<TInputParameter>>());
		}
	}

	public class ScheduleInputParameterSet1Customization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register(() =>
			{
				const MonthSchedule MONTH_SCHEDULE = MonthSchedule.January | MonthSchedule.March | MonthSchedule.June | MonthSchedule.September;
				const string WEEK_SCHEDULE = "Last";
				const DaySchedule DAY_SCHEDULE = DaySchedule.Monday | DaySchedule.Friday;

				var startTime = TimeSpan.FromHours(9);          // 09:00:00.000
				var startDate = new DateTime(2017, 1, 1);       // 2017-01-01 00:00:00.000
				return new ScheduleInputParameterSet1Format(MONTH_SCHEDULE, WEEK_SCHEDULE, DAY_SCHEDULE, startTime, startDate, null);
			});
		}
	}

	public class ArrangementForSchedulerActingOnInputSet2 : AutoDataAttribute
	{
		public ArrangementForSchedulerActingOnInputSet2()
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new SchedulerCustomization<ScheduleInputParameterSet2Format>())
				.Customize(new ScheduleInputParameterSet2Customization()))
		{

		}
	}

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
