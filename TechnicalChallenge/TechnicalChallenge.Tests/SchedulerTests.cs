using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Tests.Arrangements;
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
}
