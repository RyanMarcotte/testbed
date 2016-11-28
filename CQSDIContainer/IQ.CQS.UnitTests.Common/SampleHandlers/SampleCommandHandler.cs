using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	public class SampleCommandHandler : ICommandHandler<SampleCommand>
	{
		public static readonly object ReturnValue = typeof(void);

		public void Handle(SampleCommand command)
		{
		}
	}
}