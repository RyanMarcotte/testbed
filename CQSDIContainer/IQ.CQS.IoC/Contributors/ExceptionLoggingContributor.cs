using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Interceptors;
using IQ.CQS.IoC.Attributes;
using IQ.CQS.IoC.Constants;

namespace IQ.CQS.IoC.Contributors
{
	[InterceptorConfigurationSettingName(AppSettingsNames.IncludeExceptionLoggingInterceptor)]
	internal class ExceptionLoggingContributor : CQSInterceptorContributor<LogAnyExceptionsInterceptor>
	{
		public ExceptionLoggingContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
		}

		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.AllHandlers;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return true;
		}
	}
}
