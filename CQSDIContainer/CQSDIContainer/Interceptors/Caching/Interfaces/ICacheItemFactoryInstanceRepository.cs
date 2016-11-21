using System;
using Castle.MicroKernel;

namespace CQSDIContainer.Interceptors.Caching.Interfaces
{
	public interface ICacheItemFactoryInstanceRepository
	{
		CacheItemFactoryInfo GetCacheItemFactoryInformationForType(Type type, IKernel kernel);
	}
}