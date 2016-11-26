using System;
using System.Collections.Concurrent;
using System.Linq;
using Castle.MicroKernel;
using IQ.CQS.Caching;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Interceptors.Caching
{
	internal class CacheItemFactoryInstanceRepository : ICacheItemFactoryInstanceRepository
	{
		private static readonly ConcurrentDictionary<Type, CacheItemFactoryInfo> _cacheItemFactoryInfoLookup = new ConcurrentDictionary<Type, CacheItemFactoryInfo>();

		public CacheItemFactoryInfo GetCacheItemFactoryInformationForType(Type type, IKernel kernel)
		{
			return _cacheItemFactoryInfoLookup.GetOrAdd(type, t => GetQueryCacheItemFactory(t, kernel));
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
	}
}
