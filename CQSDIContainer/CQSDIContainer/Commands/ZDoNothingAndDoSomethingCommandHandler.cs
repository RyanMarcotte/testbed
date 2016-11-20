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
	public class ZDoNothingAndDoSomethingCommandHandler : ICommandHandler<ZDoNothingAndDoSomethingCommand>
	{
		private readonly ICommandHandler<DoNothingCommand> _doNothingCommandHandler;
		private readonly ICommandHandler<DoSomethingCommand> _doSomethingCommandHandler;

		public ZDoNothingAndDoSomethingCommandHandler(ICommandHandler<DoNothingCommand> doNothingCommandHandler, ICommandHandler<DoSomethingCommand> doSomethingCommandHandler)
		{
			_doNothingCommandHandler = doNothingCommandHandler;
			_doSomethingCommandHandler = doSomethingCommandHandler;
		}

		public void Handle(ZDoNothingAndDoSomethingCommand command)
		{
			_doNothingCommandHandler.Handle(new DoNothingCommand());
			_doSomethingCommandHandler.Handle(new DoSomethingCommand(command.NumberOfIterations));
		}
	}
}
