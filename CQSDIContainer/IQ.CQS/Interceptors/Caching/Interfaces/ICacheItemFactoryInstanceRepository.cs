using System;
using Castle.MicroKernel;

namespace IQ.CQS.Interceptors.Caching.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICacheItemFactoryInstanceRepository
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="kernel"></param>
		/// <returns></returns>
		CacheItemFactoryInfo GetCacheItemFactoryInformationForType(Type type, IKernel kernel);
	}
}