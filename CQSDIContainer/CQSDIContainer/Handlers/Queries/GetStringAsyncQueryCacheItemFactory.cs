using System;
using IQ.CQS.Caching;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetStringAsyncQueryCacheItemFactory : IQueryCacheItemFactory<GetStringAsyncQuery, string>
	{
		public string BuildKeyForQuery(GetStringAsyncQuery query)
		{
			return ":)";
		}

		public TimeSpan TimeToLive => TimeSpan.FromMinutes(5);
	}
}
