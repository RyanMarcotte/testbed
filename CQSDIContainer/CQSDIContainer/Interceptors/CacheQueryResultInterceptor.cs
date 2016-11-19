using System;
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
	public class CacheQueryResultInterceptor : IInterceptor
	{
		private readonly ICacheAside _cache;
		private readonly IKernel _kernel;

		public CacheQueryResultInterceptor(ICacheAside cache, IKernel kernel)
		{
			_cache = cache;
			_kernel = kernel;
		}

		public void Intercept(IInvocation invocation)
		{
			var info = GetQueryCacheItemFactory(invocation.InvocationTarget.GetType(), _kernel);
			if (info == null)
			{
				invocation.Proceed();
			}
			else
			{
				var invocationArgument = invocation.Arguments.FirstOrDefault();
				var buildCacheKeyForQueryMethod = info.FactoryInstance.GetType().GetMethod(nameof(IQueryCacheItemFactory<IQuery<object>, object>.BuildKeyForQuery));
				var getTimeToLiveProperty = info.FactoryInstance.GetType().GetMethod($"get_{nameof(IQueryCacheItemFactory<IQuery<object>, object>.TimeToLive)}");

				var cacheKey = $"{buildCacheKeyForQueryMethod.Invoke(info.FactoryInstance, new[] { invocationArgument })}|{info.QueryType.FullName}|{info.ResultType.FullName}";
				invocation.ReturnValue = _cache.Get(cacheKey, info.ResultType, () =>
				{
					Console.WriteLine("I'm caching something");
					invocation.Proceed();
					return invocation.ReturnValue;
				}, (TimeSpan?)getTimeToLiveProperty.Invoke(info.FactoryInstance, BindingFlags.GetProperty, null, null, null));
			}
		}

		private static QueryInfo GetQueryCacheItemFactory(Type invocationTargetType, IKernel kernel)
		{
			// make sure that the invocation target type is a query handler
			var queryHandlerInterface = invocationTargetType.GetInterfaces().FirstOrDefault();
			var queryHandlerGenericInterface = queryHandlerInterface?.GetGenericTypeDefinition();
			if (queryHandlerGenericInterface == null || (queryHandlerGenericInterface != typeof(IQueryHandler<,>) && queryHandlerGenericInterface != typeof(IAsyncQueryHandler<,>)))
				return null;

			// check if an implementation of IQueryCacheItemFactory<,> has been given for the <TQuery, TResult> pair; return it
			var queryType = queryHandlerInterface.GenericTypeArguments[0];
			var resultType = queryHandlerInterface.GenericTypeArguments[1];
			var queryCacheItemFactoryType = typeof(IQueryCacheItemFactory<,>).MakeGenericType(queryType, resultType);
			var queryCacheItemFactoryInstance = kernel.HasComponent(queryCacheItemFactoryType) ? kernel.Resolve(queryCacheItemFactoryType) : null;

			return queryCacheItemFactoryInstance != null ? new QueryInfo(queryType, resultType, queryCacheItemFactoryInstance) : null;
		}

		private class QueryInfo
		{
			public QueryInfo(Type queryType, Type resultType, object factoryInstance)
			{
				QueryType = queryType;
				ResultType = resultType;
				FactoryInstance = factoryInstance;
			}

			public Type QueryType { get; }
			public Type ResultType { get; }
			public object FactoryInstance { get; }
		}
	}
}
