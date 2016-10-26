using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.Windsor;
using CQSDIContainer.Attributes;
using CQSDIContainer.CommandDecorators;
using CQSDIContainer.Commands;
using CQSDIContainer.QueryDecorators;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer
{
	public interface ICQSFactory
	{
		ICommandHandler<TCommand> CreateCommandHandler<TCommand>() where TCommand : ICommand;
		IQueryHandler<TQuery, TResult> CreateQueryHandler<TQuery, TResult>() where TQuery : IQuery<TResult>;
	}

	public class CQSFactory : ICQSFactory
	{
		private readonly IWindsorContainer _container;

		public CQSFactory(IWindsorContainer container)
		{
			_container = container;
		}

		public ICommandHandler<TCommand> CreateCommandHandler<TCommand>()
			where TCommand : ICommand
		{
			var handler = _container.Resolve<ICommandHandler<TCommand>>();
			if (handler == null)
				throw new Exception($"No handler found for handling command '{typeof(TCommand).FullName}'!!");

			// apply decorators
			var decoratorAttributes = handler.GetType().GetCustomAttributes(false).Cast<Attribute>().ToDictionary(x => x.GetType(), x => x);
			if (decoratorAttributes.ContainsKey(typeof(LogExecutionTimeToConsoleAttribute)))
				handler = new LogExecutionTimeToConsoleCommandHandlerDecorator<TCommand>(handler);
			if (decoratorAttributes.ContainsKey(typeof(HandleCommandAsynchronouslyAttribute)))
				handler = new ExecuteAsynchronouslyCommandHandlerDecorator<TCommand>(handler);
			
			return handler;
		}

		public IQueryHandler<TQuery, TResult> CreateQueryHandler<TQuery, TResult>()
			where TQuery : IQuery<TResult>
		{
			var handler = _container.Resolve<IQueryHandler<TQuery, TResult>>();
			if (handler == null)
				throw new Exception($"No handler found for handling query '{typeof(TQuery).FullName}' with response '{typeof(TResult).FullName}'!!");

			// apply decorators
			var decoratorAttributes = handler.GetType().GetCustomAttributes(false).Cast<Attribute>().ToDictionary(x => x.GetType(), x => x);
			if (decoratorAttributes.ContainsKey(typeof(LogExecutionTimeToConsoleAttribute)))
				handler = new LogExecutionTimeToConsoleQueryHandlerDecorator<TQuery, TResult>(handler);
			
			return handler;
		}
	}
}
