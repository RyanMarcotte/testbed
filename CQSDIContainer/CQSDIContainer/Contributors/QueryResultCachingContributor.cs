using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Contributors
{
	public class QueryResultCachingContributor : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			// only queries can be cached
			var interfaces = model.Implementation.GetInterfaces();
			if (!interfaces.Any() || !interfaces.Any(CQSHandlerTypeCheckingUtility.IsQueryHandler))
				return;

			// add the interceptor
			model.Interceptors.Add(InterceptorReference.ForType<CacheQueryResultInterceptor>());
		}
	}
}
