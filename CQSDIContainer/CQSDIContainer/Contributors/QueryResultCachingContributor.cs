using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Queries.Caching;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Contributors
{
	public class QueryResultCachingContributor : CQSInterceptorContributor<CacheQueryResultInterceptor>
	{
		public QueryResultCachingContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
		}

		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.QueryHandlersOnly;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return true;
		}
	}
}
