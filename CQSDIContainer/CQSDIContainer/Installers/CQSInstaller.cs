using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CQSDIContainer.CommandDecorators.Interfaces;
using CQSDIContainer.QueryDecorators.Interfaces;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Installers
{
	public class CQSInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICommandHandler<>)).Unless(t => typeof(IDecorateCommandHandler<>).IsAssignableFrom(t)).WithServiceBase().LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncCommandHandler<>)).WithServiceBase().Unless(t => typeof(IDecorateAsyncCommandHandler<>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IResultCommandHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateResultCommandHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncResultCommandHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateAsyncResultCommandHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateQueryHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncQueryHandler<,>)).WithServiceBase().Unless(t => typeof(IDecorateAsyncQueryHandler<,>).IsAssignableFrom(t)).LifestyleSingleton())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleTransient());
		}
	}
}
