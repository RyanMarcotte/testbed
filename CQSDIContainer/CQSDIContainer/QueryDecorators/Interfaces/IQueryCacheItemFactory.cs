using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.QueryDecorators.Interfaces
{
	// // we need a constraint on TResult in order to retrieve data from the local memory cache correctly
	// (see https://github.com/AurumAS/DoubleCache/blob/master/source/DoubleCache/LocalCache/MemCache.cs Get<T> methods)
	// cannot use the non-generic method as that requires an exact type match (which will always be a concrete type; no interface storage!!)
	public interface IQueryCacheItemFactory<in TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		/// <summary>
		/// Build a key for uniquely identifying a query instance.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		string BuildKeyForQuery(TQuery query);

		/// <summary>
		/// Gets the amount of time a cache item is valid before it must be refreshed.
		/// </summary>
		TimeSpan TimeToLive { get; }
	}
}
