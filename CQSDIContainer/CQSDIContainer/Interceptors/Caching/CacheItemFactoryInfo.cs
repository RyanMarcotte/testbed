using System;

namespace CQSDIContainer.Interceptors.Caching
{
	public class CacheItemFactoryInfo
	{
		public CacheItemFactoryInfo(Type queryType, Type resultType, object factoryInstance)
		{
			QueryType = queryType;
			ResultType = resultType;
			FactoryInstance = factoryInstance;
		}

		public Type QueryType { get; }
		public Type ResultType { get; }
		public object FactoryInstance { get; }
	}
}