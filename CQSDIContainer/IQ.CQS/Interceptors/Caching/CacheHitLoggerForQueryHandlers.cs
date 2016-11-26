using System;
using IQ.CQS.Interceptors.Caching.Interfaces;

namespace IQ.CQS.Interceptors.Caching
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
