using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	public class SampleResultCommandHandlerThatFails : IResultCommandHandler<SampleCommand, int>
	{
		public static Result<Unit, int> ReturnValue => Result.Fail<Unit, int>(17);

		public Result<Unit, int> Handle(SampleCommand command)
		{
			return ReturnValue;
		}
	}
}