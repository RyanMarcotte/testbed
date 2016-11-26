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
using CQSDIContainer.Interceptors.Exceptions;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;

namespace CQSDIContainer.Interceptors
{
	public class LogExecutionTimeInterceptor : CQSInterceptorWithExceptionHandling
	{
		private static readonly ConcurrentDictionary<InvocationInstance, DateTime> _startTimeLookup = new ConcurrentDictionary<InvocationInstance, DateTime>();

		public LogExecutionTimeInterceptor(ILogExecutionTimeOfCQSHandlers executionTimeLogger)
		{
			if (executionTimeLogger == null)
				throw new ArgumentNullException(nameof(executionTimeLogger));

			ExecutionTimeLogger = executionTimeLogger;
		}

		public ILogExecutionTimeOfCQSHandlers ExecutionTimeLogger { get; }

		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			_startTimeLookup.TryAdd(invocationInstance, DateTime.UtcNow);
		}
		
		protected override void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			var end = DateTime.UtcNow;
			var threshold = TimeSpan.FromMilliseconds(componentModel.Implementation.GetCustomAttribute<LogExecutionTimeAttribute>()?.ThresholdInMilliseconds ?? LogExecutionTimeAttribute.MaximumThreshold);

			DateTime begin;
			if (!_startTimeLookup.TryGetValue(invocationInstance, out begin))
				throw new TransactionScopeNotFoundForInvocationException(invocationInstance);

			ExecutionTimeLogger.LogExecutionTime(componentModel.Implementation, end - begin, threshold);
			_startTimeLookup.TryRemove(invocationInstance, out begin);
		}
	}
}
