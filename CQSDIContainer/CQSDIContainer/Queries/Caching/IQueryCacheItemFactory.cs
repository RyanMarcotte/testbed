using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries.Caching
{
	public interface IQueryCacheItemFactory<in TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		string BuildKeyForQuery(TQuery query);

		TimeSpan TimeToLive { get; }
	}
}
