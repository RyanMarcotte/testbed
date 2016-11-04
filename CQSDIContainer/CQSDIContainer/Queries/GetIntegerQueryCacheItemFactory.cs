using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.QueryDecorators.Interfaces;

namespace CQSDIContainer.Queries
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
