using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoubleCache;
#pragma warning disable 1591 // disable XML documentation warnings

namespace IQ.CQS.Interceptors.Caching
{
	/// <summary>
	/// Null implementation of the <see cref="ICacheAside"/> interface.  Always executes data retrieval methods.
	/// </summary>
	public class NullCache : ICacheAside
	{
		public void Add<T>(string key, T item)
		{
			
		}

		public void Add<T>(string key, T item, TimeSpan? timeToLive)
		{
			
		}

		public T Get<T>(string key, Func<T> dataRetriever) where T : class
		{
			return dataRetriever.Invoke();
		}

		public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
		{
			return dataRetriever.Invoke();
		}

		public object Get(string key, Type type, Func<object> dataRetriever)
		{
			return dataRetriever.Invoke();
		}

		public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
		{
			return dataRetriever.Invoke();
		}

		public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
		{
			return await dataRetriever.Invoke();
		}

		public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
		{
			return await dataRetriever.Invoke();
		}

		public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
		{
			return await dataRetriever.Invoke();
		}

		public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
		{
			return await dataRetriever.Invoke();
		}

		public void Remove(string key)
		{
			
		}

		public TimeSpan? DefaultTtl => null;
	}
}
