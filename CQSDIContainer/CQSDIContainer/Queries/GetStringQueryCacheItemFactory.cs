using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.QueryDecorators.Interfaces;

namespace CQSDIContainer.Queries
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
