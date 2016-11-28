using System;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Commands
{
	public class DoSomethingWithResultCommandHandler : IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode>
	{
		public Result<Unit, DoSomethingWithResultCommandHandlerErrorCode> Handle(DoSomethingWithResultCommand command)
		{
			Console.WriteLine();
			Console.WriteLine("HANDLING COMMAND WITH RESULT");
			Console.WriteLine();

			if (command.Denominator == 0)
				return Result.Fail<Unit, DoSomethingWithResultCommandHandlerErrorCode>(DoSomethingWithResultCommandHandlerErrorCode.DenominatorIsZero);

			return Result.Succeed<Unit, DoSomethingWithResultCommandHandlerErrorCode>(Unit.Value);
		}
	}

	public enum DoSomethingWithResultCommandHandlerErrorCode
	{
		DenominatorIsZero
	}
}
