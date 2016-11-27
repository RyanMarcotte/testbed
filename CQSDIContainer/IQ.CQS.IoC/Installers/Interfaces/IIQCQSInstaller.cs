using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using DoubleCache;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;

namespace IQ.CQS.IoC.Installers.Interfaces
{
	/// <summary>
	/// Fluent interface for building a Castle.Windsor installer for IQ.CQS framework components.
	/// </summary>
	public interface IIQCQSInstaller
	{
		/// <summary>
		/// Configure the IQ.CQS installation to use the specified caching implementation.  The submitted type must implement the <see cref="ICacheAside"/> interface.
		/// </summary>
		/// <typeparam name="TCache">The cache type.</typeparam>
		/// <returns></returns>
		IIQCQSInstaller WithCachingImplementation<TCache>() where TCache : ICacheAside;

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogCacheHitsAndMissesForQueryHandlers"/> interface.
		/// </summary>
		/// <typeparam name="TCacheLogger">The cache logger type.</typeparam>
		/// <returns></returns>
		IIQCQSInstaller WithCustomImplementationForLoggingQueryCaching<TCacheLogger>() where TCacheLogger : ILogCacheHitsAndMissesForQueryHandlers;

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogExceptionsFromCQSHandlers"/> interface.
		/// </summary>
		/// <typeparam name="TExceptionLogger">The exception logger type.</typeparam>
		/// <returns></returns>
		IIQCQSInstaller WithCustomImplementationForExceptionLogging<TExceptionLogger>() where TExceptionLogger : ILogExceptionsFromCQSHandlers;

		/// <summary>
		/// Configure the IQ.CQS installation to use a custom implementation for logging exceptions.  The submitted type must implement the <see cref="ILogPerformanceMetricsForCQSHandlers"/> interface.
		/// </summary>
		/// <typeparam name="TPerformanceMetricsLogger">The performance metrics logger type.</typeparam>
		/// <returns></returns>
		IIQCQSInstaller WithCustomImplementationForPerformanceMetricsLogging<TPerformanceMetricsLogger>() where TPerformanceMetricsLogger : ILogPerformanceMetricsForCQSHandlers;
		
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
