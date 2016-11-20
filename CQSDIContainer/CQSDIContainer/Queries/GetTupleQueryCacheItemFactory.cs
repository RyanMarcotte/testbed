using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Queries.Caching;

namespace CQSDIContainer.Queries
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
