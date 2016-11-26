using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Interceptors;

namespace IQ.CQS.IoC.Contributors
{
	public class ExceptionLoggingContributor : CQSInterceptorContributor<LogExecutionTimeInterceptor>
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
