using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.CommandDecorators.Interfaces;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.CommandDecorators
{
	public class LogExecutionTimeToConsoleCommandHandlerDecorator<TCommand> : IDecorateCommandHandler<TCommand>
	{
		private readonly ICommandHandler<TCommand> _commandHandler;

		public LogExecutionTimeToConsoleCommandHandlerDecorator(ICommandHandler<TCommand> commandHandler)
		{
			_commandHandler = commandHandler;
		}

		public void Handle(TCommand command)
		{
			var begin = DateTime.UtcNow;
			_commandHandler.Handle(command);
			var end = DateTime.UtcNow;
			Console.WriteLine($"{_commandHandler.GetType()} measured time: {(end - begin).TotalMilliseconds} ms");
		}
	}
}
