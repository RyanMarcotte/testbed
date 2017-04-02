using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Tests.Customizations;

namespace TechnicalChallenge.Tests.Arrangements
{
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
}