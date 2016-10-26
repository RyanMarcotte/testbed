using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.CommandDecorators.Interfaces;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.CommandDecorators
{
	/// <summary>
	/// Command handler decorator that executes its wrapped command handler asynchronously.  Use for fire-and-forget operations.
	/// </summary>
	/// <remarks>
	/// IMPORTANT NOTE TO DEVELOPERS!!
	/// Command handlers wrapped with this decorator cannot be handled inside a using block.
	/// Chances are that the disposable object will be disposed before the wrapped command handler executes.
	/// The following example should make this clear.
	/// 
	/// using (var service = ApplicationServiceFactory.CreateService[ISomeService]())
	/// {
	///		// work is delegated to a background thread
	///		// service is disposed by the time the background thread executes
	///		Task.Run(() => service.Instance.DoSomething());	
	/// }
	/// </remarks>
	public class ExecuteAsynchronouslyCommandHandlerDecorator<TCommand> : IDecorateCommandHandler<TCommand>
	{
		private readonly ICommandHandler<TCommand> _commandHandler;

		public ExecuteAsynchronouslyCommandHandlerDecorator(ICommandHandler<TCommand> commandHandler)
		{
			_commandHandler = commandHandler;
		}

		public void Handle(TCommand command)
		{
			Task.Run(() => _commandHandler.Handle(command));
		}
	}
}
