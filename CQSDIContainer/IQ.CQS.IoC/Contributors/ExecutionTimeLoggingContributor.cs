using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Attributes;
using IQ.CQS.Interceptors;

namespace IQ.CQS.IoC.Contributors
{
	public class ExecutionTimeLoggingInterceptorContributor : CQSInterceptorContributor<LogExecutionTimeInterceptor>
	{
		public ExecutionTimeLoggingInterceptorContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
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
