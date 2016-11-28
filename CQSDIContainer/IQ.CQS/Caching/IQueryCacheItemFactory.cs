using System;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Caching
{
	/// <summary>
	/// Interface for cache item factories used for specifying information for the query result caching system.
	/// </summary>
	/// <typeparam name="TQuery">The query type.</typeparam>
	/// <typeparam name="TResult">The type of data returned from the query.</typeparam>
	public interface IQueryCacheItemFactory<in TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		/// <summary>
		/// Builds a cache key for the specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns></returns>
		string BuildKeyForQuery(TQuery query);

		/// <summary>
		/// The amount of time the item will live in the cache before a refresh is required.
		/// </summary>
		TimeSpan TimeToLive { get; }
	}
}
