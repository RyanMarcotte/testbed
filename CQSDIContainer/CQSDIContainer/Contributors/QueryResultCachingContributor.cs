using System;
using System.Linq;
using Castle.Core;
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
