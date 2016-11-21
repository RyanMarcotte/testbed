using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using CQSDIContainer.Interceptors;

namespace CQSDIContainer.Contributors
{
	public class ExceptionLoggingContributor : CQSInterceptorContributor<LogExecutionTimeInterceptor>
	{
		public ExceptionLoggingContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
		}

		protected override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.AllHandlers;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return true;
		}
	}
}
