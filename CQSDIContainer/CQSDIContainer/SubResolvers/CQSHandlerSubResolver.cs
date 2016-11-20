using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Resolvers;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.SubResolvers
{
	public class CQSHandlerSubResolver : ISubDependencyResolver
	{
		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			var dependencyType = dependency.TargetItemType;
			if (!dependencyType.IsGenericType)
				return false;

			return model.Services.All(x => !x.IsGenericType || !_cqsHandlerTypes.Contains(x.GetGenericTypeDefinition()));
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			throw new NotImplementedException();
		}

		private readonly IEnumerable<Type> _cqsHandlerTypes = new HashSet<Type>
			{
				typeof(IQueryHandler<,>),
				typeof(IAsyncQueryHandler<,>),
				typeof(ICommandHandler<>),
				typeof(IAsyncCommandHandler<>),
				typeof(IResultCommandHandler<,>),
				typeof(IAsyncResultCommandHandler<,>)
			};

		/*
		componentRegistration.Interceptors(InterceptorReference.ForType<LogAnyExceptionsInterceptor>()).AtIndex(0);
		componentRegistration.Interceptors(InterceptorReference.ForType<LogExecutionTimeToConsoleInterceptor>()).AtIndex(1);
		*/
	}
}
