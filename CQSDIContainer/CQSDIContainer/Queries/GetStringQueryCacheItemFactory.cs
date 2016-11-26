using System;
using IQ.CQS.Caching;

namespace IQ.CQS.Lab.Queries
{
	public class GetStringQueryCacheItemFactory : IQueryCacheItemFactory<GetStringQuery, string>
	{
		public string BuildKeyForQuery(GetStringQuery query)
		{
			return query.Value;
		}

		public TimeSpan TimeToLive => TimeSpan.FromMinutes(2);
	}
}
