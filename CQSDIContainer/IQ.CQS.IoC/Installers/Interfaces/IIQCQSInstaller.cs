using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;
using IQ.CQS.Interceptors.MetricsLogging.Interfaces;

namespace IQ.CQS.IoC.Installers.Interfaces
{
	/// <summary>
	/// Fluent interface for building a Castle.Windsor installer for IQ.CQS framework components.
	/// </summary>
	public interface IIQCQSInstaller
	{
		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogCacheHitsAndMissesForQueryHandlers"/> interface.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IIQCQSInstaller WithCustomImplementationForLoggingQueryCaching(Type type);

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogExceptionsFromCQSHandlers"/> interface.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IIQCQSInstaller WithCustomImplementationForExceptionLogging(Type type);

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogExecutionTimeOfCQSHandlers"/> interface.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IIQCQSInstaller WithCustomImplementationForPerformanceMetricsLogging(Type type);

		/// <summary>
		/// Add all custom interceptors from the specified assemblies.  The interceptor classes must be public.
		/// </summary>
		/// <param name="assemblyDescriptor">The assembly descriptor.</param>
		/// <returns></returns>
		IIQCQSInstaller WithIQCQSComponentsFromTheSpecifiedAssembly(FromAssemblyDescriptor assemblyDescriptor);

		/// <summary>
		/// Gets the configured installer.
		/// </summary>
		/// <returns></returns>
		IWindsorInstaller GetInstaller();
	}
}
