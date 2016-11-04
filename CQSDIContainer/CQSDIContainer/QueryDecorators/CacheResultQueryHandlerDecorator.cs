using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.QueryDecorators.Interfaces;
using DoubleCache;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.QueryDecorators
{
	public class CacheResultQueryHandlerDecorator<TQuery, TResult> : IDecorateQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TResult>
		where TResult : class
	{
		private readonly ICacheAside _cache;
		private readonly IQueryHandler<TQuery, TResult> _queryHandler;
		private readonly IQueryCacheItemFactory<TQuery, TResult> _cacheKeyBuilder;

		public CacheResultQueryHandlerDecorator(
			IQueryHandler<TQuery, TResult> queryHandler,
			ICacheAside cache,
			IQueryCacheItemFactory<TQuery, TResult> cacheKeyBuilder)
		{
			if (queryHandler == null)
				throw new ArgumentNullException(nameof(queryHandler));
			if (cache == null)
				throw new ArgumentNullException(nameof(cache));
			if (cacheKeyBuilder == null)
				throw new ArgumentNullException(nameof(cacheKeyBuilder));

			_cache = cache;
			_queryHandler = queryHandler;
			_cacheKeyBuilder = cacheKeyBuilder;
		}

		public TResult Handle(TQuery query)
		{
			Console.WriteLine("getting value (maybe from cache)");
			var cacheKey = $"{_cacheKeyBuilder.BuildKeyForQuery(query)}|{typeof(TQuery).FullName}|{typeof(TResult).FullName}";
			return _cache.Get(cacheKey, () => _queryHandler.Handle(query), _cacheKeyBuilder.TimeToLive);
		}
	}
}
