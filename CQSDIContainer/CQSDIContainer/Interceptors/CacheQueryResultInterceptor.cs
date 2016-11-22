using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using CQSDIContainer.Interceptors.Attributes;
using CQSDIContainer.Interceptors.Caching;
using CQSDIContainer.Interceptors.Caching.Interfaces;
using CQSDIContainer.Queries.Caching;
using DoubleCache;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Interceptors
{
	[ApplyToNestedHandlers]
	public class CacheQueryResultInterceptor : CQSInterceptor
	{
		private const string NO_QUERY_CACHE_ITEM_FACTORY_INFO_FOUND = "The interceptor cannot handle query caching if no factory information for the type exists!!  (possibly an error in QueryResultCachingContributor...)";

		private static readonly ConcurrentDictionary<object, CacheItemFactoryMethods> _cacheItemFactoryMethodLookup = new ConcurrentDictionary<object, CacheItemFactoryMethods>();
		private readonly ICacheAside _cache;
		private readonly IKernel _kernel;
		private readonly ICacheItemFactoryInstanceRepository _cacheItemFactoryInstanceRepository;
		private readonly ILogCacheHitsAndMissesForQueryHandlers _cacheLogger;

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
			_cacheLogger = cacheLogger;
		}

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
				ExecuteGetReferenceTypeFromCacheUsingReflection(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, _cacheLogger);
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

				DoCacheLogging(_cacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);
			}
		}

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
				ExecuteGetReferenceTypeAsyncFromCacheUsingReflection(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, _cacheLogger);
			else
				ExecuteGetValueTypeAsyncFromCacheUsingReflection(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory, _cacheLogger);
		}
		
		#region Common Helpers

		private static CacheItemFactoryMethods BuildMethodInfoForCacheItemFactoryInstance(object instance)
		{
			var instanceType = instance.GetType();
			var buildCacheKeyForQueryMethod = instanceType.GetMethod(nameof(IQueryCacheItemFactory<IQuery<object>, object>.BuildKeyForQuery));
			var getTimeToLiveProperty = instanceType.GetMethod($"get_{nameof(IQueryCacheItemFactory<IQuery<object>, object>.TimeToLive)}");

			return new CacheItemFactoryMethods(buildCacheKeyForQueryMethod, getTimeToLiveProperty);
		}

		private static string GetCacheKey(CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, IInvocation invocation)
		{
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
			var methodInfo = _handleAsyncMethodLookup.GetOrAdd(resultType, _handleAsyncMethodInfo.MakeGenericMethod(resultType));
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
			var methodInfo = _referenceTypeSyncFetcherMethodLookup.GetOrAdd(resultType, _getReferenceTypeFromCacheMethodInfo.MakeGenericMethod(resultType));
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
			var result = cache.GetAsync<T>(cacheKey, () =>
				{
					cacheHit = false;
					invocation.Proceed();
					ExecuteHandleAsyncWithResultUsingReflection(invocation);
					return (dynamic)invocation.ReturnValue;
				}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));

			DoCacheLogging(cacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);
			return result;
		}

		private static void ExecuteGetReferenceTypeAsyncFromCacheUsingReflection(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _referenceTypeAsyncFetcherMethodLookup.GetOrAdd(resultType, _getReferenceTypeAsyncFromCacheMethodInfo.MakeGenericMethod(resultType));
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

			DoCacheLogging(cacheLogger, cacheHit, cacheItemFactoryInfo, cacheKey);
			return result;
		}
		
		private static void ExecuteGetValueTypeAsyncFromCacheUsingReflection(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, ILogCacheHitsAndMissesForQueryHandlers cacheLogger)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _valueTypeAsyncFetcherMethodLookup.GetOrAdd(resultType, _getValueTypeAsyncFromCacheMethodInfo.MakeGenericMethod(resultType));
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
