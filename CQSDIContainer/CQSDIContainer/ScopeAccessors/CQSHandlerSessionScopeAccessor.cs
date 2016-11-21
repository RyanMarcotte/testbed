using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace CQSDIContainer.ScopeAccessors
{
	public class CQSHandlerSessionScopeAccessor : IScopeAccessor
	{
		private static readonly ConcurrentDictionary<Guid, ILifetimeScope> _collection = new ConcurrentDictionary<Guid, ILifetimeScope>();

		public ILifetimeScope GetScope(CreationContext context)
		{
			return _collection.GetOrAdd(Guid.NewGuid(), key => new CQSHandlerInvocationLifetimeScope());
		}

		public void Dispose()
		{
			foreach (var scope in _collection)
				scope.Value.Dispose();

			_collection.Clear();
		}
	}

	public class CQSHandlerInvocationLifetimeScope : ILifetimeScope
	{
		public Burden GetCachedInstance(ComponentModel model, ScopedInstanceActivationCallback createInstance)
		{
			return createInstance(delegate { });
		}

		public void Dispose()
		{
			
		}
	}
}
