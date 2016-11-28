using System;
using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.IoC.Attributes;
using IQ.CQS.IoC.Constants;

namespace IQ.CQS.IoC.Contributors
{
	/// <summary>
	/// Used to determine if the <see cref="CacheQueryResultInterceptor"/> should be applied to intercepted CQS handlers.
	/// </summary>
	[InterceptorConfigurationSettingName(AppSettingsNames.IncludeQueryResultCachingInterceptor)]
	public class QueryResultCachingContributor : CQSInterceptorContributor<CacheQueryResultInterceptor>
	{
		private readonly ICacheItemFactoryInstanceRepository _cacheItemFactoryInstanceRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryResultCachingContributor"/> class.
		/// </summary>
		/// <param name="cacheItemFactoryInstanceRepository">The cache item factory instance repository.</param>
		/// <param name="isContributingToComponentModelConstructionForNestedCQSHandlers">Indicates if the contributor is managing the application of interceptors to nested CQS handlers.</param>
		public QueryResultCachingContributor(ICacheItemFactoryInstanceRepository cacheItemFactoryInstanceRepository, bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
			if (cacheItemFactoryInstanceRepository == null)
				throw new ArgumentNullException(nameof(cacheItemFactoryInstanceRepository));

			_cacheItemFactoryInstanceRepository = cacheItemFactoryInstanceRepository;
		}

		/// <summary>
		/// Indicates which types of handlers to apply the interceptor to.
		/// </summary>
		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.QueryHandlersOnly;

		/// <summary>
		/// Indicates if the interceptor should be applied to the component model corresponding to a CQS handler.
		/// </summary>
		/// <param name="kernel">The IoC container.</param>
		/// <param name="model">The component model for the CQS handler.</param>
		/// <returns></returns>
		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return _cacheItemFactoryInstanceRepository.GetCacheItemFactoryInformationForType(model.Implementation, kernel) != null;
		}
	}
}
