using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.Proxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CQSDIContainer.Contributors;
using CQSDIContainer.Contributors.Interfaces;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.ExceptionLogging;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;
using CQSDIContainer.Interceptors.MetricsLogging;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;
using CQSDIContainer.Queries.Caching;
using CQSDIContainer.SubResolvers;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Installers
{
	public class CQSInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			// create a child container for resolving nested CQS handler dependencies
			// (a handler is nested if it must be injected as a dependency into another handler)
			var childContainer = new WindsorContainer();
			RegisterCQSHandlers(childContainer, ApplyNestedHandlerConfiguration);
			RegisterCQSContributors(container, isForNestedHandlers: true);
			container.AddChildContainer(childContainer);

			// register all CQS-related objects
			container.Kernel.Resolver.AddSubResolver(new NestedCQSHandlerResolver(childContainer.Kernel));
			RegisterCQSContributors(container, isForNestedHandlers: false);
			RegisterCQSHandlers(container, ApplyCommonConfiguration);
			container
				.Register(Component.For<ILogExceptionsFromCQSHandlers>().ImplementedBy<ExceptionLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Component.For<ILogExecutionTimeOfCQSHandlers>().ImplementedBy<ExecutionTimeLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleSingleton());
		}
		
		private static void RegisterCQSHandlers(IWindsorContainer container, Action<ComponentRegistration> componentRegistrationAction)
		{
			container
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICommandHandler<>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncCommandHandler<>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IResultCommandHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncResultCommandHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncQueryHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction));
		}

		private static void RegisterCQSContributors(IWindsorContainer container, bool isForNestedHandlers)
		{
			foreach (var contributor in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(ICQSInterceptorContributor).IsAssignableFrom(x)))
				container.Kernel.ComponentModelBuilder.AddContributor((IContributeComponentModelConstruction)Activator.CreateInstance(contributor, new object[] { isForNestedHandlers }));
		}

		private static void ApplyCommonConfiguration(ComponentRegistration componentRegistration)
		{
			
		}

		private static void ApplyNestedHandlerConfiguration(ComponentRegistration componentRegistration)
		{
			
		}
	}
}
