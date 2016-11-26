using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
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