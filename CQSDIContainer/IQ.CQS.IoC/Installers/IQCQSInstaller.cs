using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DoubleCache;
using DoubleCache.LocalCache;
using IQ.CQS.Caching;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Caching;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.Interceptors.ExceptionLogging;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;
using IQ.CQS.Interceptors.PerformanceMetricsLogging;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;
using IQ.CQS.Interceptors.TransactionScopes;
using IQ.CQS.Interceptors.TransactionScopes.Interfaces;
using IQ.CQS.IoC.Attributes;
using IQ.CQS.IoC.Constants;
using IQ.CQS.IoC.Contributors;
using IQ.CQS.IoC.Contributors.Enums;
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
		private Type _cacheLoggerForQueryHandlersType = typeof(NullCacheLoggerForQueryHandlers);
		private Type _exceptionLoggerForCQSHandlersType = typeof(NullExceptionLoggerForCQSHandlers);
		private Type _performanceMetricsLoggerForCQSHandlersType = typeof(NullPerformanceMetricsLoggerForCQSHandlers);
		private readonly List<FromAssemblyDescriptor> _userAssemblyDescriptors = new List<FromAssemblyDescriptor>();
		private NameValueCollection _configuration;

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
		/// <typeparam name="TCacheLogger">The cache logger type.</typeparam>
		/// <returns></returns>
		public IIQCQSInstaller WithCustomImplementationForLoggingQueryCaching<TCacheLogger>()
			where TCacheLogger : ILogCacheHitsAndMissesForQueryHandlers
		{
			_cacheLoggerForQueryHandlersType = typeof(TCacheLogger);
			return this;
		}

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogExceptionsFromCQSHandlers"/> interface.
		/// </summary>
		/// <typeparam name="TExceptionLogger">The exception logger type.</typeparam>
		/// <returns></returns>
		public IIQCQSInstaller WithCustomImplementationForExceptionLogging<TExceptionLogger>()
			where TExceptionLogger : ILogExceptionsFromCQSHandlers
		{
			_exceptionLoggerForCQSHandlersType = typeof(TExceptionLogger);
			return this;
		}

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogPerformanceMetricsForCQSHandlers"/> interface.
		/// </summary>
		/// <typeparam name="TPerformanceMetricsLogger">The performance metrics logger type.</typeparam>
		/// <returns></returns>
		public IIQCQSInstaller WithCustomImplementationForPerformanceMetricsLogging<TPerformanceMetricsLogger>()
			where TPerformanceMetricsLogger : ILogPerformanceMetricsForCQSHandlers
		{
			_performanceMetricsLoggerForCQSHandlersType = typeof(TPerformanceMetricsLogger);
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
		/// Configure the IQ.CQS installation to use settings from a <see cref="NameValueCollection"/> for including / excluding components during registration.
		/// </summary>
		/// <param name="configuration">The configuration represented as a <see cref="NameValueCollection"/>.</param>
		/// <returns></returns>
		public IIQCQSInstaller UsingConfiguration(NameValueCollection configuration)
		{
			_configuration = configuration;
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
			// register the cache system
			CacheType cacheType;
			if (_configuration?[AppSettingsNames.CacheType] != null && Enum.TryParse(_configuration[AppSettingsNames.CacheType], out cacheType))
			{
				switch (cacheType)
				{
					case CacheType.None:
						container.Register(Component.For<ICacheAside>().ImplementedBy<NullCache>().LifestyleSingleton());
						break;

					case CacheType.Local:
						container.Register(Component.For<ICacheAside>().ImplementedBy<MemCache>().LifestyleSingleton());
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				// use a local memory cache by default, or if there is something wrong with the configuration
				container.Register(Component.For<ICacheAside>().ImplementedBy<MemCache>().LifestyleSingleton());
			}

			// register all objects required to support IQ.CQS
			container
				.Register(Component.For<ICacheItemFactoryInstanceRepository>().ImplementedBy<CacheItemFactoryInstanceRepository>().LifestyleTransient())
				.Register(Component.For<ILogCacheHitsAndMissesForQueryHandlers>().ImplementedBy(_cacheLoggerForQueryHandlersType).LifestyleTransient())
				.Register(Component.For<ILogExceptionsFromCQSHandlers>().ImplementedBy(_exceptionLoggerForCQSHandlersType).LifestyleTransient())
				.Register(Component.For<ILogPerformanceMetricsForCQSHandlers>().ImplementedBy(_performanceMetricsLoggerForCQSHandlersType).LifestyleTransient())
				.Register(Component.For<IManageTransactionScopesForCQSHandlers>().ImplementedBy<TransactionScopeManagerForCQSHandlers>().LifestyleTransient())
				.Register(Classes.FromAssemblyContaining<InvocationInstance>().IncludeNonPublicTypes().BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient());

			// register all objects specified the user to support IQ.CQS
			foreach (var assemblyDescriptor in _userAssemblyDescriptors)
			{
				container
					.Register(assemblyDescriptor.BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
					.Register(assemblyDescriptor.BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleSingleton());
			}

			// register all contributors to a separate container
			var contributorContainer = new WindsorContainer();
			contributorContainer.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().BasedOn(typeof(ICQSInterceptorContributor)).WithServiceSelf().LifestyleTransient());
			foreach (var assemblyDescriptor in _userAssemblyDescriptors)
				contributorContainer.Register(assemblyDescriptor.BasedOn(typeof(ICQSInterceptorContributor)).WithServiceSelf().LifestyleTransient());

			// add the contributor container to the main container so that contributors can resolve dependencies on other CQS components
			// (we do not add the contributors to the main container; they were being applied to all handlers regardless of settings in configuration)
			container.AddChildContainer(contributorContainer);

			// need to add contributors before registering all handlers (the contributors affect handler registration)
			AddAllContributors(container, contributorContainer, false, _configuration);
			container.Register(AllCQSHandlers(ApplyCommonConfiguration, _userAssemblyDescriptors));

			// create a child container for resolving nested CQS handler dependencies (a handler is 'nested' if it must be injected as a dependency into another handler)
			var childContainer = new WindsorContainer();
			AddAllContributors(childContainer, contributorContainer, true, _configuration);
			childContainer.Register(AllCQSHandlers(ApplyNestedHandlerConfiguration, _userAssemblyDescriptors));

			// add the child container, then add a sub-resolver that will be responsible for resolving nested handlers
			container.AddChildContainer(childContainer);
			container.Kernel.Resolver.AddSubResolver(new NestedCQSHandlerResolver(childContainer.Kernel));

			// we're done installing everything, so remove the contributor container since it's no longer needed
			container.RemoveChildContainer(contributorContainer);
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
		/// <param name="configuration">The application configuration.</param>
		private static void AddAllContributors(IWindsorContainer containerToAddContributorsTo, IWindsorContainer containerToUseForResolution, bool isForNestedHandlers, NameValueCollection configuration)
		{
			foreach (var contributor in GetCQSContributors(containerToUseForResolution, isForNestedHandlers, configuration))
				containerToAddContributorsTo.Kernel.ComponentModelBuilder.AddContributor(contributor);
		}

		/// <summary>
		/// Resolves instances of all contributors.
		/// </summary>
		/// <param name="containerToUseForResolution">The container that will be used for resolving contributors.</param>
		/// <param name="isForNestedHandlers">Indicates if contributors are being used for constructing component models of nested handlers.</param>
		/// <param name="configuration">The application configuration.</param>
		/// <returns></returns>
		private static IEnumerable<ICQSInterceptorContributor> GetCQSContributors(IWindsorContainer containerToUseForResolution, bool isForNestedHandlers, NameValueCollection configuration)
		{
			var arguments = new Dictionary<string, object> { { "isContributingToComponentModelConstructionForNestedCQSHandlers", isForNestedHandlers } };
			foreach (var contributorType in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(ICQSInterceptorContributor).IsAssignableFrom(x)))
			{
				bool isEnabled;
				bool.TryParse(configuration?[contributorType.GetCustomAttribute<InterceptorConfigurationSettingNameAttribute>()?.SettingName ?? string.Empty], out isEnabled);

				if (isEnabled)
					yield return (ICQSInterceptorContributor)containerToUseForResolution.Resolve(contributorType, arguments);
			}
				
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
