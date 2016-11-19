using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.QueryDecorators.Interfaces;

namespace CQSDIContainer.Queries
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
