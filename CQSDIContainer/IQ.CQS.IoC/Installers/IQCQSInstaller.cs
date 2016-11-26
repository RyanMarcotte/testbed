using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using IQ.CQS.Caching;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Caching;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.Interceptors.ExceptionLogging;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;
using IQ.CQS.Interceptors.MetricsLogging;
using IQ.CQS.Interceptors.MetricsLogging.Interfaces;
using IQ.CQS.Interceptors.TransactionScopes;
using IQ.CQS.Interceptors.TransactionScopes.Interfaces;
using IQ.CQS.IoC.Contributors.Interfaces;
using IQ.CQS.IoC.Installers.Interfaces;
using IQ.CQS.IoC.SubResolvers;
using IQ.CQS.Utilities;

namespace IQ.CQS.IoC.Installers
{
	/// <summary>
	/// Castle.Windsor installer for IQ.CQS components.
	/// </summary>
	public class IQCQSInstaller : IIQCQSInstaller, IWindsorInstaller
	{
		private Type _cacheLoggerForQueryHandlers = typeof(NullCacheLoggerForQueryHandlers);
		private Type _exceptionLoggerForCQSHandlers = typeof(NullExceptionLoggerForCQSHandlers);
		private Type _performanceMetricsLoggerForCQSHandlers = typeof(NullExecutionTimeLoggerForCQSHandlers);
		private readonly List<FromAssemblyDescriptor> _userAssemblyDescriptors = new List<FromAssemblyDescriptor>();

		// hide constructor because developers must use the fluent interface to configure their IQ.CQS installation
		private IQCQSInstaller() { }

		/// <summary>
		/// Start configuration.
		/// </summary>
		/// <returns></returns>
		public static IIQCQSInstaller Build()
		{
			return new IQCQSInstaller();
		}

		#region IIQCQSInstaller implementation

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogCacheHitsAndMissesForQueryHandlers"/> interface.
		/// </summary>
		/// <param name="type">The type (must implement the <see cref="ILogCacheHitsAndMissesForQueryHandlers"/> interface).</param>
		/// <returns></returns>
		public IIQCQSInstaller WithCustomImplementationForLoggingQueryCaching(Type type)
		{
			if (!typeof(ILogCacheHitsAndMissesForQueryHandlers).IsAssignableFrom(type))
				throw new InvalidOperationException($"Submitted type must implement '{typeof(ILogCacheHitsAndMissesForQueryHandlers)}'!!");

			_cacheLoggerForQueryHandlers = type;
			return this;
		}

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogExceptionsFromCQSHandlers"/> interface.
		/// </summary>
		/// <param name="type">The type (must implement the <see cref="ILogExceptionsFromCQSHandlers"/> interface).</param>
		/// <returns></returns>
		public IIQCQSInstaller WithCustomImplementationForExceptionLogging(Type type)
		{
			if (!typeof(ILogExceptionsFromCQSHandlers).IsAssignableFrom(type))
				throw new InvalidOperationException($"Submitted type must implement '{typeof(ILogExceptionsFromCQSHandlers)}'!!");

			_exceptionLoggerForCQSHandlers = type;
			return this;
		}

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogExecutionTimeOfCQSHandlers"/> interface.
		/// </summary>
		/// <param name="type">The type (must implement the <see cref="ILogExecutionTimeOfCQSHandlers"/> interface).</param>
		/// <returns></returns>
		public IIQCQSInstaller WithCustomImplementationForPerformanceMetricsLogging(Type type)
		{
			if (!typeof(ILogExecutionTimeOfCQSHandlers).IsAssignableFrom(type))
				throw new InvalidOperationException($"Submitted type must implement '{typeof(ILogExecutionTimeOfCQSHandlers)}'!!");

			_performanceMetricsLoggerForCQSHandlers = type;
			return this;
		}

		/// <summary>
		/// Add all custom interceptors from the specified assemblies.  The interceptor classes must be public.
		/// </summary>
		/// <param name="assemblyDescriptor">The assembly descriptor.</param>
		/// <returns></returns>
		public IIQCQSInstaller WithIQCQSComponentsFromTheSpecifiedAssembly(FromAssemblyDescriptor assemblyDescriptor)
		{
			_userAssemblyDescriptors.Add(assemblyDescriptor);
			return this;
		}

		/// <summary>
		/// Gets the configured installer.
		/// </summary>
		/// <returns></returns>
		public IWindsorInstaller GetInstaller()
		{
			return this;
		}

		#endregion

		#region IWindsorInstaller implementation

		/// <summary>
		/// Perform the installation.
		/// </summary>
		/// <param name="container">The container that will contain the installed components.</param>
		/// <param name="store">The contract used by the kernel to obtain external configuration for the components and facilities.</param>
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			// register all objects required to support IQ.CQS
			container.Install(new CacheInstaller());
			container
				.Register(Component.For<ICacheItemFactoryInstanceRepository>().ImplementedBy<CacheItemFactoryInstanceRepository>().LifestyleTransient())
				.Register(Component.For<ILogCacheHitsAndMissesForQueryHandlers>().ImplementedBy(_cacheLoggerForQueryHandlers).LifestyleTransient())
				.Register(Component.For<ILogExceptionsFromCQSHandlers>().ImplementedBy(_exceptionLoggerForCQSHandlers).LifestyleTransient())
				.Register(Component.For<ILogExecutionTimeOfCQSHandlers>().ImplementedBy(_performanceMetricsLoggerForCQSHandlers).LifestyleTransient())
				.Register(Component.For<IManageTransactionScopesForCQSHandlers>().ImplementedBy<TransactionScopeManagerForCQSHandlers>().LifestyleTransient())
				.Register(Classes.FromAssemblyContaining<InvocationInstance>().BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICQSInterceptorContributor)).WithServiceSelf().LifestyleTransient());

			// register all objects specified the user to support IQ.CQS
			foreach (var assemblyDescriptor in _userAssemblyDescriptors)
				container
					.Register(assemblyDescriptor.BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
					.Register(assemblyDescriptor.BasedOn(typeof(ICQSInterceptorContributor)).WithServiceSelf().LifestyleTransient())
					.Register(assemblyDescriptor.BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleSingleton());

			// need to add contributors before registering all handlers (the contributors affect handler registration)
			AddAllContributors(container, container, false);
			container.Register(AllCQSHandlers(ApplyCommonConfiguration, _userAssemblyDescriptors));

			// create a child container for resolving nested CQS handler dependencies (a handler is 'nested' if it must be injected as a dependency into another handler)
			var childContainer = new WindsorContainer();
			AddAllContributors(childContainer, container, true);
			childContainer.Register(AllCQSHandlers(ApplyNestedHandlerConfiguration, _userAssemblyDescriptors));

			// add the child container, then add a sub-resolver that will be responsible for resolving nested handlers
			container.AddChildContainer(childContainer);
			container.Kernel.Resolver.AddSubResolver(new NestedCQSHandlerResolver(childContainer.Kernel));
		}

		/// <summary>
		/// Retrieve the collection of all CQS handler registrations.
		/// </summary>
		/// <param name="componentRegistrationAction">The component registration action.</param>
		/// <param name="userAssemblyDescriptors"></param>
		/// <returns></returns>
		private static IRegistration[] AllCQSHandlers(Action<ComponentRegistration> componentRegistrationAction, IEnumerable<FromAssemblyDescriptor> userAssemblyDescriptors)
		{
			return GetAllCQSHandlerRegistrations(componentRegistrationAction, userAssemblyDescriptors).ToArray();
		}

		private static IEnumerable<IRegistration> GetAllCQSHandlerRegistrations(Action<ComponentRegistration> componentRegistrationAction, IEnumerable<FromAssemblyDescriptor> userAssemblyDescriptors)
		{
			var registrations = new List<IRegistration>();
			
			foreach (var userAssemblyDescriptor in userAssemblyDescriptors)
				registrations.AddRange(CQSHandlerTypeCheckingUtility.SupportedHandlerTypes.Select(supportedHandlerType => userAssemblyDescriptor.BasedOn(supportedHandlerType).WithServiceBase().LifestyleSingleton().Configure(componentRegistrationAction)));

			return registrations;
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

		#endregion
	}
}
