using System;
using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Caching.Interfaces;

namespace IQ.CQS.IoC.Contributors
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
