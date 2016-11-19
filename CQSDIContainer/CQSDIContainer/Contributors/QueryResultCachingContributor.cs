using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using CQSDIContainer.Interceptors;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Contributors
{
	public class QueryResultCachingContributor : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			// only queries can be cached
			var interfaces = model.Implementation.GetInterfaces();
			if (!interfaces.Any() || !interfaces.Any(IsQueryHandler))
				return;

			// the interceptor for caching query results must have been registered already
			if (kernel.GetHandlers(typeof(IInterceptor)).All(x => x.ComponentModel.Implementation != typeof(CacheQueryResultInterceptor)))
				throw new ComponentNotFoundException(typeof(CacheQueryResultInterceptor));

			// add the interceptor
			model.Interceptors.Add(InterceptorReference.ForType<CacheQueryResultInterceptor>());
		}

		private static bool IsQueryHandler(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IQueryHandler<,>) || genericTypeDefinition == typeof(IAsyncQueryHandler<,>);
		}
	}
}
