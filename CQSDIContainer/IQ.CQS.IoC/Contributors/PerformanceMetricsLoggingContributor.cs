using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Attributes;
using IQ.CQS.Interceptors;
using IQ.CQS.IoC.Attributes;
using IQ.CQS.IoC.Constants;

namespace IQ.CQS.IoC.Contributors
{
	[InterceptorConfigurationSettingName(AppSettingsNames.IncludePerformanceMetricsLoggingInterceptor)]
	internal class PerformanceMetricsLoggingContributor : CQSInterceptorContributor<LogPerformanceMetricsInterceptor>
	{
		public PerformanceMetricsLoggingContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
			
		}

		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.AllHandlers;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			// interceptor is opt-in
			return model.Implementation.GetCustomAttribute<LogExecutionTimeAttribute>() != null;
		}
	}
}
