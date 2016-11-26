using System;

namespace IQ.CQS.Interceptors.Caching.Interfaces
{
	public interface ILogCacheHitsAndMissesForQueryHandlers
	{
		void LogCacheHit(Type queryType, Type resultType, string cacheKey);
		void LogCacheMiss(Type queryType, Type resultType, string cacheKey);
	}
}
