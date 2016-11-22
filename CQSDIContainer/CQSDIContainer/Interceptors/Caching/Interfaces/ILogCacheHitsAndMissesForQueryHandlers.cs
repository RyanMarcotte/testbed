using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Interceptors.Caching.Interfaces
{
	public interface ILogCacheHitsAndMissesForQueryHandlers
	{
		void LogCacheHit(Type queryType, Type resultType, string cacheKey);
		void LogCacheMiss(Type queryType, Type resultType, string cacheKey);
	}
}
