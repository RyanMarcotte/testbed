using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Attributes;
using IQ.CQS.Interceptors;
using IQ.CQS.IoC.Attributes;
using IQ.CQS.IoC.Constants;

namespace IQ.CQS.IoC.Contributors
{
	[InterceptorConfigurationSettingName(AppSettingsNames.IncludeTransactionScopeInterceptor)]
	internal class TransactionScopeContributor : CQSInterceptorContributor<TransactionScopeInterceptor>
	{
		public TransactionScopeContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{

		}

		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.CommandHandlersOnly;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			// interceptor is opt-out
			return model.Implementation.GetCustomAttribute<NoTransactionScopeAttribute>() == null;
		}
	}
}
