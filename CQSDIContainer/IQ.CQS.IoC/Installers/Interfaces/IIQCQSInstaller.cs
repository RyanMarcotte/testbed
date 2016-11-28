using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using DoubleCache;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;
using IQ.CQS.IoC.Contributors.Enums;

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
		/// Configure the IQ.CQS installation to include a set of custom components based on the specified type.
		/// </summary>
		/// <param name="type">The type used as basis for registration.</param>
		/// <param name="serviceRegistrationType">The service registration type.</param>
		/// <param name="lifestyleType">The lifestyle type to use for all registered components of this type.</param>
		/// <returns></returns>
		IIQCQSInstaller WithCustomIQCQSComponentsBasedOn(Type type, ServiceRegistrationType serviceRegistrationType, LifestyleType lifestyleType);

		/// <summary>
		/// Configure the IQ.CQS installation to use settings from a <see cref="NameValueCollection"/> for including / excluding components during registration.
		/// </summary>
		/// <param name="configuration">The configuration represented as a <see cref="NameValueCollection"/>.</param>
		/// <returns></returns>
		IIQCQSInstaller UsingConfiguration(NameValueCollection configuration);

		/// <summary>
		/// Gets the configured installer.
		/// </summary>
		/// <returns></returns>
		IWindsorInstaller GetInstaller();
	}
}
