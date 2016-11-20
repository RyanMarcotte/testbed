using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Attributes;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Commands
{
	[LogExecutionTime]
	public class DoNothingAndDoSomethingCommandHandler : ICommandHandler<DoNothingAndDoSomethingCommand>
	{
		private readonly ICommandHandler<DoNothingCommand> _doNothingCommandHandler;
		private readonly ICommandHandler<DoSomethingCommand> _doSomethingCommandHandler;

		public DoNothingAndDoSomethingCommandHandler(ICommandHandler<DoNothingCommand> doNothingCommandHandler, ICommandHandler<DoSomethingCommand> doSomethingCommandHandler)
		{
			_doNothingCommandHandler = doNothingCommandHandler;
			_doSomethingCommandHandler = doSomethingCommandHandler;
		}

		public void Handle(DoNothingAndDoSomethingCommand command)
		{
			_doNothingCommandHandler.Handle(new DoNothingCommand());
			_doSomethingCommandHandler.Handle(new DoSomethingCommand(command.NumberOfIterations));
		}
	}
}
