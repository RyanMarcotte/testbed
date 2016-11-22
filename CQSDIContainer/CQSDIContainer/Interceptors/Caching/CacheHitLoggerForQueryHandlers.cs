using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Interceptors.Caching.Interfaces;

namespace CQSDIContainer.Interceptors.Caching
{
	public class CacheHitLoggerForQueryHandlers : ILogCacheHitsAndMissesForQueryHandlers
	{
		public void LogCacheHit(Type queryType, Type resultType, string cacheKey)
		{
			Console.WriteLine($"CACHE HIT for <{queryType}, {resultType}> (key = {cacheKey})");
		}

		public void LogCacheMiss(Type queryType, Type resultType, string cacheKey)
		{
			Console.WriteLine($"CACHE MISS for <{queryType}, {resultType}> (key = '{cacheKey}')");
		}
	}
}
