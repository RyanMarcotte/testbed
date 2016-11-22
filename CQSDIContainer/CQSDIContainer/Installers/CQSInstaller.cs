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
				.Register(Component.For<ILogCacheHitsAndMissesForQueryHandlers>().ImplementedBy<CacheHitLoggerForQueryHandlers>().LifestyleTransient())
				.Register(Component.For<ILogExceptionsFromCQSHandlers>().ImplementedBy<ExceptionLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Component.For<ILogExecutionTimeOfCQSHandlers>().ImplementedBy<ExecutionTimeLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICQSInterceptorContributor)).WithServiceSelf().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleSingleton());

			// need to add contributors before registering all handlers (the contributors affect handler registration)
			AddAllContributors(container, container, false);
			container.Register(AllCQSHandlers(ApplyCommonConfiguration));

			// create a child container for resolving nested CQS handler dependencies (a handler is 'nested' if it must be injected as a dependency into another handler)
			var childContainer = new WindsorContainer();
			AddAllContributors(childContainer, container, true);
			childContainer.Register(AllCQSHandlers(ApplyNestedHandlerConfiguration));

			// add the child container, then add a sub-resolver that will be responsible for resolving nested handlers
			container.AddChildContainer(childContainer);
			container.Kernel.Resolver.AddSubResolver(new NestedCQSHandlerResolver(childContainer.Kernel));
		}

		/// <summary>
		/// Retrieve the collection of all CQS handler registrations.
		/// </summary>
		/// <param name="componentRegistrationAction">The component registration action.</param>
		/// <returns></returns>
		private static IRegistration[] AllCQSHandlers(Action<ComponentRegistration> componentRegistrationAction)
		{
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

		/// <summary>
		/// Add all contributors to the container.
		/// </summary>
		/// <param name="containerToAddContributorsTo">The container that all contributors will be added to.</param>
		/// <param name="containerToUseForResolution">The container that will be used for resolving contributors.</param>
		/// <param name="isForNestedHandlers">Indicates if contributors are being used for constructing component models of nested handlers.</param>
		private static void AddAllContributors(IWindsorContainer containerToAddContributorsTo, IWindsorContainer containerToUseForResolution, bool isForNestedHandlers)
		{
			foreach (var contributor in GetCQSContributors(containerToUseForResolution, isForNestedHandlers))
				containerToAddContributorsTo.Kernel.ComponentModelBuilder.AddContributor(contributor);
		}

		/// <summary>
		/// Resolves instances of all contributors.
		/// </summary>
		/// <param name="containerToUseForResolution">The container that will be used for resolving contributors.</param>
		/// <param name="isForNestedHandlers">Indicates if contributors are being used for constructing component models of nested handlers.</param>
		/// <returns></returns>
		private static IEnumerable<ICQSInterceptorContributor> GetCQSContributors(IWindsorContainer containerToUseForResolution, bool isForNestedHandlers)
		{
			var arguments = new Dictionary<string, object> { { "isContributingToComponentModelConstructionForNestedCQSHandlers", isForNestedHandlers } };
			foreach (var contributor in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(ICQSInterceptorContributor).IsAssignableFrom(x)))
				yield return (ICQSInterceptorContributor)containerToUseForResolution.Resolve(contributor, arguments);
		}

		/// <summary>
		/// Apply any custom configuration to non-nested handlers.
		/// </summary>
		/// <param name="componentRegistration">The component registration.</param>
		private static void ApplyCommonConfiguration(ComponentRegistration componentRegistration)
		{
			
		}

		/// <summary>
		/// Apply any custom configuration to nested handlers.
		/// </summary>
		/// <param name="componentRegistration">The component registration.</param>
		private static void ApplyNestedHandlerConfiguration(ComponentRegistration componentRegistration)
		{
			
		}
	}
}
