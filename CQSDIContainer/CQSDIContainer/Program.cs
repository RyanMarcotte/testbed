using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using IQ.CQS.IoC.Installers;
using IQ.CQS.Lab.Caching;
using IQ.CQS.Lab.ExceptionLogging;
using IQ.CQS.Lab.PerformanceMetrics;

namespace IQ.CQS.Lab
{
	internal class Program
	{
		// http://tommarien.github.io/blog/2013/05/11/i-command-you/
		private static void Main(string[] args)
		{
			var container = new WindsorContainer();
			container.AddFacility<TypedFactoryFacility>();
			container.Install(IQCQSInstaller.Build()
				.WithCustomImplementationForExceptionLogging(typeof(ExceptionLoggerForCQSHandlers))
				.WithCustomImplementationForLoggingQueryCaching(typeof(CacheLoggerForQueryHandlers))
				.WithCustomImplementationForPerformanceMetricsLogging(typeof(ExecutionTimeLoggerForCQSHandlers))
				.WithIQCQSComponentsFromTheSpecifiedAssembly(Classes.FromThisAssembly())
				.GetInstaller());

			container.Install(FromAssembly.This());

			var service = container.Resolve<IRQApplicationServiceMock>();
			service.DoStuff().GetAwaiter().GetResult();
		}
	}
}
