using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
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