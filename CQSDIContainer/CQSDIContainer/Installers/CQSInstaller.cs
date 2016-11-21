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
using CQSDIContainer.Interceptors.Caching;
using CQSDIContainer.Interceptors.Caching.Interfaces;
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
			// register all objects required to support CQS
			container
				.Register(Component.For<ICacheItemFactoryInstanceRepository>().ImplementedBy<CacheItemFactoryInstanceRepository>().LifestyleTransient())
				.Register(Component.For<ILogExceptionsFromCQSHandlers>().ImplementedBy<ExceptionLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Component.For<ILogExecutionTimeOfCQSHandlers>().ImplementedBy<ExecutionTimeLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICQSInterceptorContributor)).WithServiceSelf().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleSingleton())
				.Register(AllCQSContributorsAndHandlers(container, container, false, ApplyCommonConfiguration));

			// create a child container for resolving nested CQS handler dependencies
			// (a handler is nested if it must be injected as a dependency into another handler)
			var childContainer = new WindsorContainer();
			childContainer.Register(AllCQSContributorsAndHandlers(childContainer, container, true, ApplyNestedHandlerConfiguration));

			// add the child container, then add a sub-resolver that will be responsible for resolving nested handlers
			container.AddChildContainer(childContainer);
			container.Kernel.Resolver.AddSubResolver(new NestedCQSHandlerResolver(childContainer.Kernel));
		}

		private static IRegistration[] AllCQSContributorsAndHandlers(IWindsorContainer container, IWindsorContainer containerToUseForResolution, bool isForNestedHandlers, Action<ComponentRegistration> componentRegistrationAction)
		{
			foreach (var contributor in GetCQSContributors(containerToUseForResolution, isForNestedHandlers))
				container.Kernel.ComponentModelBuilder.AddContributor(contributor);

			return GetAllCQSHandlerRegistrations(componentRegistrationAction).ToArray();
		}

		private static IEnumerable<IRegistration> GetAllCQSHandlerRegistrations(Action<ComponentRegistration> componentRegistrationAction)
		{
			yield return Classes.FromThisAssembly().BasedOn(typeof(ICommandHandler<>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction);
			yield return Classes.FromThisAssembly().BasedOn(typeof(IAsyncCommandHandler<>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction);
			yield return Classes.FromThisAssembly().BasedOn(typeof(IResultCommandHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction);
			yield return Classes.FromThisAssembly().BasedOn(typeof(IAsyncResultCommandHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction);
			yield return Classes.FromThisAssembly().BasedOn(typeof(IQueryHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction);
			yield return Classes.FromThisAssembly().BasedOn(typeof(IAsyncQueryHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction);
		}

		private static IEnumerable<ICQSInterceptorContributor> GetCQSContributors(IWindsorContainer containerToUseForResolution, bool isForNestedHandlers)
		{
			var arguments = new Dictionary<string, object> { { "isContributingToComponentModelConstructionForNestedCQSHandlers", isForNestedHandlers } };
			foreach (var contributor in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(ICQSInterceptorContributor).IsAssignableFrom(x)))
				yield return (ICQSInterceptorContributor)containerToUseForResolution.Resolve(contributor, arguments);
		}

		private static void ApplyCommonConfiguration(ComponentRegistration componentRegistration)
		{
			
		}

		private static void ApplyNestedHandlerConfiguration(ComponentRegistration componentRegistration)
		{
			
		}
	}
}
