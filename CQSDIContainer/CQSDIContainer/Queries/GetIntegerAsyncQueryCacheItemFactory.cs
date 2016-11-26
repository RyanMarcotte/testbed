using System;
using IQ.CQS.Caching;

namespace IQ.CQS.Lab.Queries
{
	public class GetIntegerAsyncQueryCacheItemFactory : IQueryCacheItemFactory<GetIntegerAsyncQuery, int>
	{
		public string BuildKeyForQuery(GetIntegerAsyncQuery query)
		{
			return $"{query.Value}";
		}

		public TimeSpan TimeToLive => TimeSpan.FromMinutes(5);
	}
}
