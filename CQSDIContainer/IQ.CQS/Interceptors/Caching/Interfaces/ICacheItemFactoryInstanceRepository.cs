using System;
using Castle.MicroKernel;

namespace IQ.CQS.Interceptors.Caching.Interfaces
{
	internal interface ICacheItemFactoryInstanceRepository
	{
		CacheItemFactoryInfo GetCacheItemFactoryInformationForType(Type type, IKernel kernel);
	}
}