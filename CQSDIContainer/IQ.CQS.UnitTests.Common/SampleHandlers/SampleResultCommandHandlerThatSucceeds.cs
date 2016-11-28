using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	public class SampleResultCommandHandlerThatSucceeds : IResultCommandHandler<SampleCommand, int>
	{
		public static Result<Unit, int> ReturnValue => Result.Succeed<Unit, int>(Unit.Value);

		public Result<Unit, int> Handle(SampleCommand command)
		{
			return ReturnValue;
		}
	}
}