using System;
using IQ.CQS.Interceptors.Caching.Interfaces;

namespace IQ.CQS.Interceptors.Caching
{
	internal class NullCacheLoggerForQueryHandlers : ILogCacheHitsAndMissesForQueryHandlers
	{
		public void LogCacheHit(Type queryType, Type resultType, string cacheKey)
		{
			
		}

		public void LogCacheMiss(Type queryType, Type resultType, string cacheKey)
		{
			
		}
	}
}
