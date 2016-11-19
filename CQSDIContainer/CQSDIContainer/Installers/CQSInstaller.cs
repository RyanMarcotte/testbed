using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CQSDIContainer.CommandDecorators.Interfaces;
using CQSDIContainer.QueryDecorators.Interfaces;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Installers
{
	public interface ICommandHandlerFactory<TCommand>
	{
		ICommandHandler<TCommand> Create();
		void Release(ICommandHandler<TCommand> instance);
	}

	public interface IAsyncCommandHandlerFactory<TCommand>
	{
		IAsyncCommandHandler<TCommand> Create();
		void Release(IAsyncCommandHandler<TCommand> instance);
	}

	public interface IResultCommandHandlerFactory<TCommand, TError>
	{
		IResultCommandHandler<TCommand, TError> Create();
		void Release(IResultCommandHandler<TCommand, TError> instance);
	}

	public interface IAsyncResultCommandHandlerFactory<TCommand, TError>
	{
		IAsyncResultCommandHandler<TCommand, TError> Create();
		void Release(IAsyncResultCommandHandler<TCommand, TError> instance);
	}

	public interface IQueryHandlerFactory<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		IQueryHandler<TQuery, TResult> Create();
		void Release(IQueryHandler<TQuery, TResult> instance);
	}

	public interface IAsyncQueryHandlerFactory<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		IAsyncQueryHandler<TQuery, TResult> Create();
		void Release(IAsyncQueryHandler<TQuery, TResult> instance);
	}

	public class CQSInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			/*container.Register(Component.For(typeof(ICommandHandlerFactory<>)).AsFactory());
			container.Register(Component.For(typeof(IAsyncCommandHandlerFactory<>)).AsFactory());
			container.Register(Component.For(typeof(IResultCommandHandlerFactory<,>)).AsFactory());
			container.Register(Component.For(typeof(IAsyncResultCommandHandlerFactory<,>)).AsFactory());
			container.Register(Component.For(typeof(IQueryHandlerFactory<,>)).AsFactory());
			container.Register(Component.For(typeof(IAsyncQueryHandlerFactory<,>)).AsFactory());*/

			container.Kernel.Resolver.AddSubResolver(new QueryHandlerResolver());
			container
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICommandHandler<>)).Unless(t => typeof(IDecorateCommandHandler<>).IsAssignableFrom(t)).WithServiceBase().LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncCommandHandler<>)).WithServiceBase().Unless(t => typeof(IDecorateAsyncCommandHandler<>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IResultCommandHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateResultCommandHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncResultCommandHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateAsyncResultCommandHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateQueryHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncQueryHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateAsyncQueryHandler<,>).IsAssignableFrom(t)).LifestyleSingleton());
		}
	}

	public class QueryHandlerResolver : ISubDependencyResolver
	{
		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return dependency.TargetType.IsGenericType && dependency.TargetType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			throw new NotImplementedException();
		}
	}
}
