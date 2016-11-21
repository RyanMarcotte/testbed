using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Caching;
using CQSDIContainer.Interceptors.Caching.Interfaces;
using CQSDIContainer.Queries.Caching;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Contributors
{
	public class QueryResultCachingContributor : CQSInterceptorContributor<CacheQueryResultInterceptor>
	{
		private readonly ICacheItemFactoryInstanceRepository _cacheItemFactoryInstanceRepository;

		public QueryResultCachingContributor(ICacheItemFactoryInstanceRepository cacheItemFactoryInstanceRepository, bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
			if (cacheItemFactoryInstanceRepository == null)
				throw new ArgumentNullException(nameof(cacheItemFactoryInstanceRepository));

			_cacheItemFactoryInstanceRepository = cacheItemFactoryInstanceRepository;
		}

		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.QueryHandlersOnly;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return _cacheItemFactoryInstanceRepository.GetCacheItemFactoryInformationForType(model.Implementation, kernel) != null;
		}
	}
}
