using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using DoubleCache;
using DoubleCache.LocalCache;
using Ploeh.AutoFixture;

namespace IQ.CQS.UnitTests._Customizations
{
	public class CacheAsideCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<ICacheAside>(() => new TestCache(Guid.NewGuid()));
		}

		/// <summary>
		/// A <see cref="ICacheAside"/> implementation used for unit tests.
		/// </summary>
		/// <remarks>
		/// I've just copied the source code from https://github.com/AurumAS/DoubleCache/blob/master/source/DoubleCache/LocalCache/MemCache.cs with a minor tweak.
		/// Since the original MemCache uses MemoryCache.Default, items in the cache would persist between tests.  This uses a unique MemoryCache instance on the backend instead.
		/// </remarks>
		private class TestCache : ICacheAside
		{
			private readonly MemoryCache _cache;

			public TestCache(Guid id, TimeSpan? defaultTtl = null)
			{
				DefaultTtl = defaultTtl;
				_cache = new MemoryCache(id.ToString());
			}

			public TimeSpan? DefaultTtl { get; }

			public void Add<T>(string key, T item)
			{
				var policy = new CacheItemPolicy();

				if (DefaultTtl.HasValue)
					policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(DefaultTtl.Value);
				_cache.Set(key, item, policy);
			}

			public void Add<T>(string key, T item, TimeSpan? timeToLive)
			{
				var policy = new CacheItemPolicy();

				if (timeToLive.HasValue)
					policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(timeToLive.Value);
				_cache.Set(key, item, policy);
			}

			public T Get<T>(string key, Func<T> dataRetriever) where T : class
			{
				return Get(key, dataRetriever, DefaultTtl);
			}

			public T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class
			{
				var item = _cache.Get(key) as T;
				if (item != null)
					return item;
				{
					item = dataRetriever.Invoke();
					Add(key, item, timeToLive);
				}
				return item;
			}

			public object Get(string key, Type type, Func<object> dataRetriever)
			{
				return Get(key, type, dataRetriever, DefaultTtl);
			}

			public object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive)
			{
				var item = _cache.Get(key);
				if (item != null && item.GetType() == type)
					return item;

				item = dataRetriever.Invoke();
				Add(key, item, timeToLive);
				return item.GetType() == type ? item : null;
			}

			public Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever)
			{
				return GetAsync(key, type, dataRetriever, DefaultTtl);
			}

			public async Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive)
			{
				var item = _cache.Get(key);
				if (item != null && item.GetType() == type)
					return item;

				item = await dataRetriever.Invoke();
				Add(key, item, timeToLive);
				return item.GetType() == type ? item : null;
			}

			public Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class
			{
				return GetAsync(key, dataRetriever, DefaultTtl);
			}

			public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class
			{
				var item = _cache.Get(key) as T;
				if (item != null)
					return item;
				{
					item = await dataRetriever.Invoke();
					Add(key, item, timeToLive);
				}
				return item;
			}

			public void Remove(string key)
			{
				_cache.Remove(key);
			}
		}
	}
}
