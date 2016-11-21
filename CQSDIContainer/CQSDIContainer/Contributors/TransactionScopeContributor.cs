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
	public class TransactionScopeContributor : CQSInterceptorContributor<TransactionScopeInterceptor>
	{
		public TransactionScopeContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{

		}

		protected override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.CommandHandlersOnly;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			// interceptor is opt-out
			return model.Implementation.GetCustomAttribute<NoTransactionScopeAttribute>() == null;
		}
	}
}
