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
	public class ExecutionTimeLoggingContributor : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			// only CQS handlers can be logged
			var interfaces = model.Implementation.GetInterfaces();
			if (!interfaces.Any() || !interfaces.Any(CQSHandlerTypeCheckingUtility.IsCQSHandler))
				return;

			// add the interceptor (if the handler had LogExecutionTimeAttribute applied to it)
			if (model.Implementation.GetCustomAttribute<LogExecutionTimeAttribute>() != null)
				model.Interceptors.Add(InterceptorReference.ForType<LogExecutionTimeToConsoleInterceptor>());
		}
	}
}
