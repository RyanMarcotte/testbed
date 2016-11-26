using System;
using IQ.CQS.Caching;

namespace IQ.CQS.Lab.Queries
{
	public class GetIntegerQueryCacheItemFactory : IQueryCacheItemFactory<GetIntegerQuery, int>
	{
		public string BuildKeyForQuery(GetIntegerQuery query)
		{
			return $"{query.ID}";
		}

		public TimeSpan TimeToLive => TimeSpan.FromMinutes(15);
	}
}
