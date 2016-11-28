using System;

namespace IQ.CQS.Interceptors.Caching.Interfaces
{
	/// <summary>
	/// Interface for an object responsible for recording cache hits and misses.
	/// </summary>
	public interface ILogCacheHitsAndMissesForQueryHandlers
	{
		/// <summary>
		/// Log a cache hit.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		void LogCacheHit(Type queryType, Type resultType, string cacheKey);

		/// <summary>
		/// Log a cache miss.
		/// </summary>
		/// <param name="queryType">The query type.</param>
		/// <param name="resultType">The result type.</param>
		/// <param name="cacheKey">The cache key.</param>
		void LogCacheMiss(Type queryType, Type resultType, string cacheKey);
	}
}
