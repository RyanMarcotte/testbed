﻿using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Interceptors;
using IQ.CQS.IoC.Attributes;
using IQ.CQS.IoC.Constants;

namespace IQ.CQS.IoC.Contributors
{
	/// <summary>
	/// Used to determine if the <see cref="LogAnyExceptionsInterceptor"/> should be applied to intercepted CQS handlers.
	/// </summary>
	[InterceptorConfigurationSettingName(AppSettingsNames.IncludeExceptionLoggingInterceptor)]
	public class ExceptionLoggingContributor : CQSInterceptorContributor<LogAnyExceptionsInterceptor>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionLoggingContributor"/> class.
		/// </summary>
		/// <param name="isContributingToComponentModelConstructionForNestedCQSHandlers">Indicates if the contributor is managing the application of interceptors to nested CQS handlers.</param>
		public ExceptionLoggingContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
		}

		/// <summary>
		/// Indicates which types of handlers to apply the interceptor to.
		/// </summary>
		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.AllHandlers;

		/// <summary>
		/// Indicates if the interceptor should be applied to the component model corresponding to a CQS handler.
		/// </summary>
		/// <param name="kernel">The IoC container.</param>
		/// <param name="model">The component model for the CQS handler.</param>
		/// <returns></returns>
		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return true;
		}
	}
}
