using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.Windsor;
using CQSDIContainer.Attributes;
using CQSDIContainer.CommandDecorators;
using CQSDIContainer.Commands;
using CQSDIContainer.QueryDecorators;
using CQSDIContainer.QueryDecorators.Interfaces;
using DoubleCache;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer
{
	public interface ICQSFactory
	{
		ICommandHandler<TCommand> CreateCommandHandler<TCommand>() where TCommand : ICommand;
		IQueryHandler<TQuery, TResult> CreateQueryHandler<TQuery, TResult>() where TQuery : IQuery<TResult>;
	}

	public class CQSFactory : ICQSFactory
	{
		private readonly IWindsorContainer _container;
		private readonly ICacheAside _cache;

		private readonly ConcurrentDictionary<Tuple<Type, Type>, Delegate> _delegateCache;
		private readonly MethodInfo _getHandlerWithCachingForReferenceTypeResultMethodInfo;
		private readonly MethodInfo _getHandlerWithCachingForValueTypeResultMethodInfo;

		public CQSFactory(IWindsorContainer container)
		{
			_container = container;
			_cache = container.Resolve<ICacheAside>();

			// we've added these to aid with decorating query handlers with caching functionality
			_delegateCache = new ConcurrentDictionary<Tuple<Type, Type>, Delegate>();
			
			_getHandlerWithCachingForReferenceTypeResultMethodInfo = typeof(CQSFactory).GetMethod(nameof(GetHandlerWithCachingForReferenceTypeResult), BindingFlags.NonPublic | BindingFlags.Instance);
			if (_getHandlerWithCachingForReferenceTypeResultMethodInfo == null)
				throw new ArgumentNullException(nameof(_getHandlerWithCachingForReferenceTypeResultMethodInfo));
			
			_getHandlerWithCachingForValueTypeResultMethodInfo = typeof(CQSFactory).GetMethod(nameof(GetHandlerWithCachingForValueTypeResult), BindingFlags.NonPublic | BindingFlags.Instance);
			if (_getHandlerWithCachingForValueTypeResultMethodInfo == null)
				throw new ArgumentNullException(nameof(_getHandlerWithCachingForValueTypeResultMethodInfo));
		}

		public ICommandHandler<TCommand> CreateCommandHandler<TCommand>()
			where TCommand : ICommand
		{
			var handler = _container.Resolve<ICommandHandler<TCommand>>();
			if (handler == null)
				throw new Exception($"No handler found for handling command '{typeof(TCommand).FullName}'!!");

			// apply decorators
			var decoratorAttributes = handler.GetType().GetCustomAttributes(false).Cast<Attribute>().ToDictionary(x => x.GetType(), x => x);
			if (decoratorAttributes.ContainsKey(typeof(LogExecutionTimeToConsoleAttribute)))
				handler = new LogExecutionTimeToConsoleCommandHandlerDecorator<TCommand>(handler);
			if (decoratorAttributes.ContainsKey(typeof(HandleCommandAsynchronouslyAttribute)))
				handler = new ExecuteAsynchronouslyCommandHandlerDecorator<TCommand>(handler);
			
			return handler;
		}

		public IQueryHandler<TQuery, TResult> CreateQueryHandler<TQuery, TResult>()
			where TQuery : IQuery<TResult>
		{
			var handler = _container.Resolve<IQueryHandler<TQuery, TResult>>();
			if (handler == null)
				throw new Exception($"No handler found for handling query '{typeof(TQuery).FullName}' with response '{typeof(TResult).FullName}'!!");

			// need to use reflection to retrieve a caching decorator due to the type constraints on GetHandlerWithCaching methods
			// we use type constraints because the ICacheAside implementation doesn't play well with storing interfaces using the non-generic Get method (it will attempt to match on the concrete type instead)
			// the generic Get method will attempt to convert an object to the requested type, so that method will play well with storing instances of interfaces
			if (typeof(TResult).IsClass)
			{
				var key = Tuple.Create(typeof(TQuery), typeof(TResult));
				var @delegate = _delegateCache.GetOrAdd(key, t2 => Delegate.CreateDelegate(typeof(Func<IQueryHandler<TQuery, TResult>, IQueryHandler<TQuery, TResult>>), this, _getHandlerWithCachingForReferenceTypeResultMethodInfo.MakeGenericMethod(typeof(TQuery), typeof(TResult))));
				handler = ((Func<IQueryHandler<TQuery, TResult>, IQueryHandler<TQuery, TResult>>) @delegate).Invoke(handler);
			}
			else
			{
				var key = Tuple.Create(typeof(TQuery), typeof(TResult));
				var @delegate = _delegateCache.GetOrAdd(key, t2 => Delegate.CreateDelegate(typeof(Func<IQueryHandler<TQuery, TResult>, IQueryHandler<TQuery, TResult>>), this, _getHandlerWithCachingForValueTypeResultMethodInfo.MakeGenericMethod(typeof(TQuery), typeof(TResult))));
				handler = ((Func<IQueryHandler<TQuery, TResult>, IQueryHandler<TQuery, TResult>>) @delegate).Invoke(handler);
			}

			// apply decorators
			var decoratorAttributes = handler.GetType().GetCustomAttributes(false).Cast<Attribute>().ToDictionary(x => x.GetType(), x => x);
			if (decoratorAttributes.ContainsKey(typeof(LogExecutionTimeToConsoleAttribute)))
				handler = new LogExecutionTimeToConsoleQueryHandlerDecorator<TQuery, TResult>(handler);
			
			return handler;
		}

		private IQueryHandler<TQuery, TResult> GetHandlerWithCachingForReferenceTypeResult<TQuery, TResult>(IQueryHandler<TQuery, TResult> queryHandler)
			where TQuery : IQuery<TResult>
			where TResult : class
		{
			// first check to see if a cache item factory is available for this [TQuery, TResult] pair...
			if (!_container.Kernel.HasComponent(typeof(IQueryCacheItemFactory<TQuery, TResult>)))
				return queryHandler;

			// if so, then decorate the query handler with it
			var cacheKeyFactory = _container.Resolve<IQueryCacheItemFactory<TQuery, TResult>>();
			return cacheKeyFactory != null ? new CacheReferenceTypeResultQueryHandlerDecorator<TQuery, TResult>(queryHandler, _cache, cacheKeyFactory) : queryHandler;
		}

		private IQueryHandler<TQuery, TResult> GetHandlerWithCachingForValueTypeResult<TQuery, TResult>(IQueryHandler<TQuery, TResult> queryHandler)
			where TQuery : IQuery<TResult>
			where TResult : struct
		{
			// first check to see if a cache item factory is available for this [TQuery, TResult] pair...
			if (!_container.Kernel.HasComponent(typeof(IQueryCacheItemFactory<TQuery, TResult>)))
				return queryHandler;

			// if so, then decorate the query handler with it
			var cacheKeyFactory = _container.Resolve<IQueryCacheItemFactory<TQuery, TResult>>();
			return cacheKeyFactory != null ? new CacheValueTypeResultQueryHandlerDecorator<TQuery, TResult>(queryHandler, _cache, cacheKeyFactory) : queryHandler;
		}
	}
}
