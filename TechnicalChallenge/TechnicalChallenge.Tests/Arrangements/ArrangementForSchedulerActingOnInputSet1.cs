using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Tests.Customizations;

namespace TechnicalChallenge.Tests.Arrangements
{
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
}