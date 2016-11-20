using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using CQSDIContainer.Attributes;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Contributors
{
	public class TransactionScopeContributor : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			// only CQS command handlers can be wrapped in a transaction scope
			var interfaces = model.Implementation.GetInterfaces();
			if (!interfaces.Any() || !interfaces.Any(CQSHandlerTypeCheckingUtility.IsCommandHandler))
				return;

			// add the interceptor (unless the handler had NoTransactionScopeAttribute applied to it)
			if (model.Implementation.GetCustomAttribute<NoTransactionScopeAttribute>() == null)
				model.Interceptors.Add(InterceptorReference.ForType<TransactionScopeInterceptor>());
		}
	}
}
