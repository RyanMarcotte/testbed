using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using DoubleCache;
using IQ.CQS.Caching;
using IQ.CQS.Interceptors.Attributes;
using IQ.CQS.Interceptors.Caching;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Interceptors
{
	/// <summary>
	/// Interceptor for query handlers.  Caches the result of those queries.
	/// </summary>
	[ApplyToNestedHandlers]
	public class CacheQueryResultInterceptor : CQSInterceptor
	{
		private const string NO_QUERY_CACHE_ITEM_FACTORY_INFO_FOUND = "The interceptor cannot handle query caching if no factory information for the type exists!!  (possibly an error in QueryResultCachingContributor...)";

		private static readonly ConcurrentDictionary<object, CacheItemFactoryMethods> _cacheItemFactoryMethodLookup = new ConcurrentDictionary<object, CacheItemFactoryMethods>();
		private readonly ICacheAside _cache;
		private readonly IKernel _kernel;
		private readonly ICacheItemFactoryInstanceRepository _cacheItemFactoryInstanceRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheQueryResultInterceptor"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="kernel">The Castle.Windsor kernel used for creating instances of cache item factories.</param>
		/// <param name="cacheItemFactoryInstanceRepository">The cache item factory instance repository.</param>
		/// <param name="cacheLogger">The cache logger.</param>
		public CacheQueryResultInterceptor(ICacheAside cache, IKernel kernel, ICacheItemFactoryInstanceRepository cacheItemFactoryInstanceRepository, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
		{
			if (cache == null)
				throw new ArgumentNullException(nameof(cache));
			if (kernel == null)
				throw new ArgumentNullException(nameof(kernel));
			if (cacheItemFactoryInstanceRepository == null)
				throw new ArgumentNullException(nameof(cacheItemFactoryInstanceRepository));
			if (cacheLogger == null)
				throw new ArgumentNullException(nameof(cacheLogger));

			_cache = cache;
			_kernel = kernel;
			_cacheItemFactoryInstanceRepository = cacheItemFactoryInstanceRepository;
			
			CacheLogger = cacheLogger;
		}

		/// <summary>
		/// Get the cache logger.
		/// </summary>
		public ILogCacheHitsAndMissesForQueryHandlers CacheLogger { get; }

		/// <summary>
		/// Interception logic for synchronous handlers.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			var cacheItemFactoryInfo = _cacheItemFactoryInstanceRepository.GetCacheItemFactoryInformationForType(invocation.InvocationTarget.GetType(), _kernel);
			if (cacheItemFactoryInfo == null)
				throw new InvalidOperationException(NO_QUERY_CACHE_ITEM_FACTORY_INFO_FOUND);
			
			// create key for retrieving the item from the cache
			var cacheItemFactory = _cacheItemFactoryMethodLookup.GetOrAdd(cacheItemFactoryInfo.FactoryInstance, BuildMethodInfoForCacheItemFactoryInstance);
			var cacheKey = GetCacheKey(cacheItemFactoryInfo, cacheItemFactory, invocation);
				
			// retrieve the item from the cache
			if (cacheItemFactoryInfo.ResultType.IsClass)
			{
				ExecuteGetReferenceTypeFromCacheUsingReflection(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, CacheLogger);
			}
			else
			{
				bool cacheHit = true;
				invocation.ReturnValue = _cache.Get(cacheKey, cacheItemFactoryInfo.ResultType, () =>
					{
						cacheHit = false;
						invocation.Proceed();
						return invocation.ReturnValue;
					}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));

				DoCacheLogging(CacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);
			}
		}

		/// <summary>
		/// Interception logic for asynchronous handlers.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="methodType">The asynchronous method type.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
		{
			if (methodType == AsynchronousMethodType.Action)
				throw new InvalidOperationException("All async queries should return Task<T>!!");

			var cacheItemFactoryInfo = _cacheItemFactoryInstanceRepository.GetCacheItemFactoryInformationForType(invocation.InvocationTarget.GetType(), _kernel);
			if (cacheItemFactoryInfo == null)
				throw new InvalidOperationException(NO_QUERY_CACHE_ITEM_FACTORY_INFO_FOUND);
			
			// create key for retrieving the item from the cache
			var cacheItemFactory = _cacheItemFactoryMethodLookup.GetOrAdd(cacheItemFactoryInfo.FactoryInstance, BuildMethodInfoForCacheItemFactoryInstance);
			var cacheKey = GetCacheKey(cacheItemFactoryInfo, cacheItemFactory, invocation);

			// retrieve the item from the cache
			if (cacheItemFactoryInfo.ResultType.IsClass)
				ExecuteGetReferenceTypeAsyncFromCacheUsingReflection(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, CacheLogger);
			else
				ExecuteGetValueTypeAsyncFromCacheUsingReflection(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, CacheLogger);
		}
		
		#region Common Helpers

		private static CacheItemFactoryMethods BuildMethodInfoForCacheItemFactoryInstance(object instance)
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var instanceType = instance.GetType();
			var buildCacheKeyForQueryMethod = instanceType.GetMethod(nameof(IQueryCacheItemFactory<IQuery<object>, object>.BuildKeyForQuery));
			var getTimeToLiveProperty = instanceType.GetMethod($"get_{nameof(IQueryCacheItemFactory<IQuery<object>, object>.TimeToLive)}");

			return new CacheItemFactoryMethods(buildCacheKeyForQueryMethod, getTimeToLiveProperty);
		}

		private static string GetCacheKey(CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, IInvocation invocation)
		{
			if (cacheItemFactoryInfo == null)
				throw new ArgumentNullException(nameof(cacheItemFactoryInfo));
			if (cacheItemFactory == null)
				throw new ArgumentNullException(nameof(cacheItemFactory));
			if (invocation == null)
				throw new ArgumentNullException(nameof(invocation));

			var cacheKeyForCQSHandlerArgument = cacheItemFactory.BuildKeyForQueryMethod.Invoke(cacheItemFactoryInfo.FactoryInstance, new[] { invocation.Arguments.FirstOrDefault() });
			return $"{cacheKeyForCQSHandlerArgument}|{cacheItemFactoryInfo.QueryType.FullName}|{cacheItemFactoryInfo.ResultType.FullName}";
		}

		private static void DoCacheLogging(ILogCacheHitsAndMissesForQueryHandlers logger, bool isCacheHit, CacheItemFactoryInfo queryInfo, string cacheKey)
		{
			if (isCacheHit)
				logger.LogCacheHit(queryInfo.QueryType, queryInfo.ResultType, cacheKey);
			else
				logger.LogCacheMiss(queryInfo.QueryType, queryInfo.ResultType, cacheKey);
		}

		#region Async Method Handling

		private static readonly ConcurrentDictionary<Type, MethodInfo> _handleAsyncMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task)
		{
			return await task;
		}

		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _handleAsyncMethodLookup.GetOrAdd(resultType, t => _handleAsyncMethodInfo.MakeGenericMethod(t));
			invocation.ReturnValue = methodInfo.Invoke(null, new[] { invocation.ReturnValue });
		}

		#endregion

		#endregion // common helpers

		#region Synchronous Cache Retrieval Helpers

		private static readonly ConcurrentDictionary<Type, MethodInfo> _referenceTypeSyncFetcherMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _getReferenceTypeFromCacheMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(GetReferenceTypeFromCache), BindingFlags.Static | BindingFlags.NonPublic);

		private static T GetReferenceTypeFromCache<T>(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
			where T : class
		{
			bool cacheHit = true;
			var result = cache.Get<T>(cacheKey, () =>
				{
					cacheHit = false;
					invocation.Proceed();
					return (dynamic)invocation.ReturnValue;
				}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));

			DoCacheLogging(cacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);
			return result;
		}

		private static void ExecuteGetReferenceTypeFromCacheUsingReflection(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
		{
			var resultType = invocation.Method.ReturnType;
			var methodInfo = _referenceTypeSyncFetcherMethodLookup.GetOrAdd(resultType, t => _getReferenceTypeFromCacheMethodInfo.MakeGenericMethod(t));
			invocation.ReturnValue = methodInfo.Invoke(null, new object[] { invocation, cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, cacheLogger });
		}

		#endregion

		#region Asynchronous Cache Retrieval Helpers

		#region ... for reference types

		private static readonly ConcurrentDictionary<Type, MethodInfo> _referenceTypeAsyncFetcherMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _getReferenceTypeAsyncFromCacheMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(GetReferenceTypeAsyncFromCache), BindingFlags.Static | BindingFlags.NonPublic);

		private static Task<T> GetReferenceTypeAsyncFromCache<T>(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
			where T : class
		{
			bool cacheHit = true;
			var result = cache.GetAsync<T>(cacheKey, async () =>
				{
					cacheHit = false;
					invocation.Proceed();
					ExecuteHandleAsyncWithResultUsingReflection(invocation);
					return await (dynamic)invocation.ReturnValue;
				}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));

			result.GetAwaiter().GetResult();
			if (result.Exception == null)
				DoCacheLogging(cacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);

			return result;
		}

		private static void ExecuteGetReferenceTypeAsyncFromCacheUsingReflection(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _referenceTypeAsyncFetcherMethodLookup.GetOrAdd(resultType, t => _getReferenceTypeAsyncFromCacheMethodInfo.MakeGenericMethod(t));
			invocation.ReturnValue = methodInfo.Invoke(null, new object[] { invocation, cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, cacheLogger });
		}

		#endregion

		#region ... for value types

		private static readonly ConcurrentDictionary<Type, MethodInfo> _valueTypeAsyncFetcherMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _getValueTypeAsyncFromCacheMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(GetValueTypeAsyncFromCache), BindingFlags.Static | BindingFlags.NonPublic);

		private static async Task<T> ConvertTaskToAppropriateType<T>(Task<object> task)
			where T : struct
		{
			var result = await task;
			return (T)result;
		}

		private static Task<T> GetValueTypeAsyncFromCache<T>(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
			where T : struct
		{
			// cache.GetAsync returns Task<object>, so must convert to appropriate Task<T>
			bool cacheHit = true;
			var result = ConvertTaskToAppropriateType<T>(cache.GetAsync(cacheKey, cacheItemFactoryInfo.ResultType, async () =>
				{
					cacheHit = false;
					invocation.Proceed();
					ExecuteHandleAsyncWithResultUsingReflection(invocation);
					return await (dynamic)invocation.ReturnValue;
				}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null)));

			result.GetAwaiter().GetResult();
			if (result.Exception == null)
				DoCacheLogging(cacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);

			return result;
		}
		
		private static void ExecuteGetValueTypeAsyncFromCacheUsingReflection(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _valueTypeAsyncFetcherMethodLookup.GetOrAdd(resultType, t => _getValueTypeAsyncFromCacheMethodInfo.MakeGenericMethod(t));
			invocation.ReturnValue = methodInfo.Invoke(null, new object[] { invocation, cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, cacheLogger });
		}

		#endregion

		#endregion

		#region Internal Classes

		private class CacheItemFactoryMethods
		{
			public CacheItemFactoryMethods(MethodInfo buildKeyForQueryMethod, MethodInfo getTimeToLiveProperty)
			{
				BuildKeyForQueryMethod = buildKeyForQueryMethod;
				GetTimeToLiveProperty = getTimeToLiveProperty;
			}

			public MethodInfo BuildKeyForQueryMethod { get; }
			public MethodInfo GetTimeToLiveProperty { get; }
		}

		#endregion
	}
}
