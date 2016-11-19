using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using CQSDIContainer.QueryDecorators.Interfaces;
using DoubleCache;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Interceptors
{
	// http://www.codeproject.com/Articles/1080517/Aspect-Oriented-Programming-using-Interceptors-wit#ArticleInterceptAsync
	// http://stackoverflow.com/questions/28099669/intercept-async-method-that-returns-generic-task-via-dynamicproxy
	public class CacheQueryResultInterceptor : CQSInterceptor
	{
		private static readonly ConcurrentDictionary<Type, CacheItemFactoryInfo> _cacheItemFactoryInfoLookup = new ConcurrentDictionary<Type, CacheItemFactoryInfo>();
		private static readonly ConcurrentDictionary<object, CacheItemFactoryMethods> _cacheItemFactoryMethodLookup = new ConcurrentDictionary<object, CacheItemFactoryMethods>();
		private readonly ICacheAside _cache;
		private readonly IKernel _kernel;

		public CacheQueryResultInterceptor(ICacheAside cache, IKernel kernel)
		{
			_cache = cache;
			_kernel = kernel;
		}

		protected override void InterceptSync(IInvocation invocation)
		{
			var cacheItemFactoryInfo = _cacheItemFactoryInfoLookup.GetOrAdd(invocation.InvocationTarget.GetType(), type => GetQueryCacheItemFactory(type, _kernel));
			if (cacheItemFactoryInfo == null)
			{
				// no caching is required
				invocation.Proceed();
			}
			else
			{
				// create key for retrieving the item from the cache
				var cacheItemFactory = _cacheItemFactoryMethodLookup.GetOrAdd(cacheItemFactoryInfo.FactoryInstance, BuildMethodInfoForCacheItemFactoryInstance);
				var cacheKey = GetCacheKey(cacheItemFactoryInfo, cacheItemFactory, invocation);
				
				// retrieve the item from the cache
				if (cacheItemFactoryInfo.ResultType.IsClass)
				{
					ExecuteGetReferenceTypeFromCache(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory);
				}
				else
				{
					invocation.ReturnValue = _cache.Get(cacheKey, cacheItemFactoryInfo.ResultType, () =>
					{
						Console.WriteLine($"I'm caching something for cache key {cacheKey}");
						invocation.Proceed();
						return invocation.ReturnValue;
					}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));
				}
			}
		}

		protected override void InterceptAsync(IInvocation invocation)
		{
			var cacheItemFactoryInfo = _cacheItemFactoryInfoLookup.GetOrAdd(invocation.InvocationTarget.GetType(), type => GetQueryCacheItemFactory(type, _kernel));
			if (cacheItemFactoryInfo == null)
			{
				// no caching is required
				invocation.Proceed();
				ExecuteHandleAsyncWithResultUsingReflection(invocation);
			}
			else
			{
				// create key for retrieving the item from the cache
				var cacheItemFactory = _cacheItemFactoryMethodLookup.GetOrAdd(cacheItemFactoryInfo.FactoryInstance, BuildMethodInfoForCacheItemFactoryInstance);
				var cacheKey = GetCacheKey(cacheItemFactoryInfo, cacheItemFactory, invocation);

				// retrieve the item from the cache
				if (cacheItemFactoryInfo.ResultType.IsClass)
					ExecuteGetReferenceTypeAsyncFromCache(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory);
				else
					ExecuteGetValueTypeAsyncFromCache(invocation, _cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory);
			}
		}

		#region Common Helpers

		private static string GetCacheKey(CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory, IInvocation invocation)
		{
			var cacheKeyForCQSHandlerArgument = cacheItemFactory.BuildKeyForQueryMethod.Invoke(cacheItemFactoryInfo.FactoryInstance, new[] { invocation.Arguments.FirstOrDefault() });
			return $"{cacheKeyForCQSHandlerArgument}|{cacheItemFactoryInfo.QueryType.FullName}|{cacheItemFactoryInfo.ResultType.FullName}";
		}

		private static CacheItemFactoryInfo GetQueryCacheItemFactory(Type invocationTargetType, IKernel kernel)
		{
			// make sure that the invocation target type is a query handler
			var queryHandlerInterface = invocationTargetType.GetInterfaces().FirstOrDefault();
			var queryHandlerGenericInterface = queryHandlerInterface?.GetGenericTypeDefinition();
			if (queryHandlerGenericInterface == null || (queryHandlerGenericInterface != typeof(IQueryHandler<,>) && queryHandlerGenericInterface != typeof(IAsyncQueryHandler<,>)))
				return null;

			// check if an implementation of IQueryCacheItemFactory<,> has been given for the <TQuery, TResult> pair
			// we assume that query cache item factories have been registered as singletons
			var queryType = queryHandlerInterface.GenericTypeArguments[0];
			var resultType = queryHandlerInterface.GenericTypeArguments[1];
			var queryCacheItemFactoryType = typeof(IQueryCacheItemFactory<,>).MakeGenericType(queryType, resultType);
			var queryCacheItemFactoryInstance = kernel.HasComponent(queryCacheItemFactoryType) ? kernel.Resolve(queryCacheItemFactoryType) : null;

			return queryCacheItemFactoryInstance != null ? new CacheItemFactoryInfo(queryType, resultType, queryCacheItemFactoryInstance) : null;
		}

		private static CacheItemFactoryMethods BuildMethodInfoForCacheItemFactoryInstance(object instance)
		{
			var instanceType = instance.GetType();
			var buildCacheKeyForQueryMethod = instanceType.GetMethod(nameof(IQueryCacheItemFactory<IQuery<object>, object>.BuildKeyForQuery));
			var getTimeToLiveProperty = instanceType.GetMethod($"get_{nameof(IQueryCacheItemFactory<IQuery<object>, object>.TimeToLive)}");

			return new CacheItemFactoryMethods(buildCacheKeyForQueryMethod, getTimeToLiveProperty);
		}

		#region

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task)
		{
			return await task;
		}

		private static readonly MethodInfo _handleAsyncMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var mi = _handleAsyncMethodInfo.MakeGenericMethod(resultType);
			invocation.ReturnValue = mi.Invoke(null, new[] { invocation.ReturnValue });
		}

		#endregion

		#endregion

		#region Synchronous Cache Retrieval Helpers

		private static T GetReferenceTypeFromCacheWithReflection<T>(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory)
			where T : class
		{
			return cache.Get<T>(cacheKey, () =>
			{
				Console.WriteLine($"I'm caching something for cache key '{cacheKey}'");
				invocation.Proceed();
				return (dynamic)invocation.ReturnValue;
			}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));
		}

		private static readonly MethodInfo _getReferenceTypeFromCacheMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(GetReferenceTypeFromCacheWithReflection), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteGetReferenceTypeFromCache(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory)
		{
			var resultType = invocation.Method.ReturnType;
			var mi = _getReferenceTypeFromCacheMethodInfo.MakeGenericMethod(resultType);
			invocation.ReturnValue = mi.Invoke(null, new object[] { invocation, cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory });
		}

		#endregion

		#region Asynchronous Cache Retrieval Helpers

		private static Task<T> GetReferenceTypeAsyncFromCacheWithReflection<T>(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory)
			where T : class
		{
			return cache.GetAsync<T>(cacheKey, () =>
				{
					Console.WriteLine($"I'm caching something for cache key '{cacheKey}'");
					invocation.Proceed();
					ExecuteHandleAsyncWithResultUsingReflection(invocation);
					return (dynamic)invocation.ReturnValue;
				}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));
		}

		private static readonly MethodInfo _getReferenceTypeAsyncFromCacheMethodInfo = typeof(CacheQueryResultInterceptor).GetMethod(nameof(GetReferenceTypeAsyncFromCacheWithReflection), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteGetReferenceTypeAsyncFromCache(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var mi = _getReferenceTypeAsyncFromCacheMethodInfo.MakeGenericMethod(resultType);
			invocation.ReturnValue = mi.Invoke(null, new object[] { invocation, cache, cacheKey, cacheItemFactoryInfo, cacheItemFactory });
		}

		private static void ExecuteGetValueTypeAsyncFromCache(IInvocation invocation, ICacheAside cache, string cacheKey, CacheItemFactoryInfo cacheItemFactoryInfo, CacheItemFactoryMethods cacheItemFactory)
		{
			invocation.ReturnValue = cache.GetAsync(cacheKey, cacheItemFactoryInfo.ResultType, () =>
				{
					Console.WriteLine($"I'm caching something for cache key '{cacheKey}'");
					invocation.Proceed();
					ExecuteHandleAsyncWithResultUsingReflection(invocation);
					return (dynamic)invocation.ReturnValue;
				}, (TimeSpan?)cacheItemFactory.GetTimeToLiveProperty.Invoke(cacheItemFactoryInfo.FactoryInstance, BindingFlags.GetProperty, null, null, null));
		}

		#endregion

		#region Internal Classes

		private class CacheItemFactoryInfo
		{
			public CacheItemFactoryInfo(Type queryType, Type resultType, object factoryInstance)
			{
				QueryType = queryType;
				ResultType = resultType;
				FactoryInstance = factoryInstance;
			}

			public Type QueryType { get; }
			public Type ResultType { get; }
			public object FactoryInstance { get; }
		}

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
