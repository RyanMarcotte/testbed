using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.ExceptionLogging;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;
using CQSDIContainer.Interceptors.MetricsLogging;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;
using CQSDIContainer.Interceptors.Session;
using CQSDIContainer.Interceptors.Session.Interfaces;
using CQSDIContainer.Queries.Caching;
using CQSDIContainer.ScopeAccessors;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Installers
{
	public class CQSInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			foreach (var contributor in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && typeof(IContributeComponentModelConstruction).IsAssignableFrom(x)))
				container.Kernel.ComponentModelBuilder.AddContributor((IContributeComponentModelConstruction)Activator.CreateInstance(contributor));;
			
			container
				//.Register(Component.For<ICQSHandlerSession>().ImplementedBy<CQSHandlerSession>().LifestyleScoped<CQSHandlerSessionScopeAccessor>())
				.Register(Component.For<ILogExceptionsFromCQSHandlers>().ImplementedBy<ExceptionLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Component.For<ILogExecutionTimeOfCQSHandlers>().ImplementedBy<ExecutionTimeLoggerForCQSHandlers>().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IInterceptor)).WithServiceBase().LifestyleTransient())
				.Register(Classes.FromThisAssembly().BasedOn(typeof(ICommandHandler<>)).WithServiceBase().LifestyleSingleton().Configure(ApplyCommonInterceptors))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncCommandHandler<>)).WithServiceBase().LifestyleSingleton().Configure(ApplyCommonInterceptors))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IResultCommandHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(ApplyCommonInterceptors))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncResultCommandHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(ApplyCommonInterceptors))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(ApplyCommonInterceptors))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IAsyncQueryHandler<,>)).WithServiceBase().LifestyleSingleton().Configure(ApplyCommonInterceptors))
				.Register(Classes.FromThisAssembly().BasedOn(typeof(IQueryCacheItemFactory<,>)).WithServiceBase().LifestyleSingleton());
		}

		private static void ApplyCommonInterceptors(ComponentRegistration componentRegistration)
		{
			var i = componentRegistration.Interceptors(InterceptorReference.ForType<LogAnyExceptionsInterceptor>()).Last;
			var j = componentRegistration.Interceptors(InterceptorReference.ForType<DisableInterceptionForNestedHandlersInterceptor>()).Last;
		}
	}
}
