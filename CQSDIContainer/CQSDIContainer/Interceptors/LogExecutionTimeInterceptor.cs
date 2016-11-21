using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Attributes;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;

namespace CQSDIContainer.Interceptors
{
	public class LogExecutionTimeInterceptor : CQSInterceptorWithExceptionHandling
	{
		private readonly ILogExecutionTimeOfCQSHandlers _executionTimeLogger;
		private DateTime _begin;

		public LogExecutionTimeInterceptor(ILogExecutionTimeOfCQSHandlers executionTimeLogger)
		{
			if (executionTimeLogger == null)
				throw new ArgumentNullException(nameof(executionTimeLogger));

			_executionTimeLogger = executionTimeLogger;
		}

		protected override void OnBeginInvocation(ComponentModel componentModel)
		{
			_begin = DateTime.UtcNow;
		}
		
		protected override void OnEndInvocation(ComponentModel componentModel)
		{
			var end = DateTime.UtcNow;
			var threshold = TimeSpan.FromMilliseconds(componentModel.Implementation.GetCustomAttribute<LogExecutionTimeAttribute>()?.ThresholdInMilliseconds ?? LogExecutionTimeAttribute.MaximumThreshold);
			_executionTimeLogger.LogExecutionTime(componentModel.Implementation, end - _begin, threshold);
		}
	}
}
