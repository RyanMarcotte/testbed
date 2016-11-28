using System;

namespace IQ.CQS.Interceptors.Caching
{
	/// <summary>
	/// Holds information about a cache item factory.
	/// </summary>
	public class CacheItemFactoryInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CacheItemFactoryInfo"/> class.
		/// </summary>
		/// <param name="queryType">The query parameter object type.</param>
		/// <param name="resultType">The result object type.</param>
		/// <param name="factoryInstance">An instance of the factory used to generate cache items.</param>
		public CacheItemFactoryInfo(Type queryType, Type resultType, object factoryInstance)
		{
			QueryType = queryType;
			ResultType = resultType;
			FactoryInstance = factoryInstance;
		}

		/// <summary>
		/// Gets the query parameter object type.
		/// </summary>
		public Type QueryType { get; }

		/// <summary>
		/// Gets the result object type.
		/// </summary>
		public Type ResultType { get; }

		/// <summary>
		/// Gets an instance of the factory used to generate cache items.
		/// </summary>
		public object FactoryInstance { get; }
	}
}