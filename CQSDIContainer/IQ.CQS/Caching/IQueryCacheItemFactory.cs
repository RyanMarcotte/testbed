using System;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Caching
{
	public interface IQueryCacheItemFactory<in TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		string BuildKeyForQuery(TQuery query);

		TimeSpan TimeToLive { get; }
	}
}
