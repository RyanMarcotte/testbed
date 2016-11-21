using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using CQSDIContainer.Attributes;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Contributors
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
