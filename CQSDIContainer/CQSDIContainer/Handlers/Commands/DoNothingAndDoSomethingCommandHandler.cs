using System;
using IQ.CQS.Attributes;
using IQ.CQS.Lab.Handlers.Queries;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Handlers.Commands
{
	[LogExecutionTime(5)]
	public class DoNothingAndDoSomethingCommandHandler : ICommandHandler<DoNothingAndDoSomethingCommand>
	{
		private readonly IQueryHandler<GetIntegerQuery, int> _getIntegerQueryHandler;
		private readonly ICommandHandler<DoNothingCommand> _doNothingCommandHandler;
		private readonly ICommandHandler<DoSomethingCommand> _doSomethingCommandHandler;

		public DoNothingAndDoSomethingCommandHandler(
			IQueryHandler<GetIntegerQuery, int> getIntegerQueryHandler,
			ICommandHandler<DoNothingCommand> doNothingCommandHandler,
			ICommandHandler<DoSomethingCommand> doSomethingCommandHandler)
		{
			_getIntegerQueryHandler = getIntegerQueryHandler;
			_doNothingCommandHandler = doNothingCommandHandler;
			_doSomethingCommandHandler = doSomethingCommandHandler;
		}

		public void Handle(DoNothingAndDoSomethingCommand command)
		{
			var result = _getIntegerQueryHandler.Handle(new GetIntegerQuery(command.NumberOfIterations));
			if (result == 3)
			{
				Console.WriteLine("result was 3, so aborting");
				return;
			}

			_doNothingCommandHandler.Handle(new DoNothingCommand());
			_doSomethingCommandHandler.Handle(new DoSomethingCommand(command.NumberOfIterations));
		}
	}
}
