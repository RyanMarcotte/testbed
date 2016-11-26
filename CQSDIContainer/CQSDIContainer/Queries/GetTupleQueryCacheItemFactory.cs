using System;
using IQ.CQS.Caching;

namespace IQ.CQS.Lab.Queries
{
	public class GetTupleQueryCacheItemFactory : IQueryCacheItemFactory<GetTupleQuery, Tuple<int, string, int>>
	{
		public string BuildKeyForQuery(GetTupleQuery query)
		{
			return $"{query.ID}|{query.Version}";
		}

		public TimeSpan TimeToLive => TimeSpan.FromMinutes(5);
	}
}
