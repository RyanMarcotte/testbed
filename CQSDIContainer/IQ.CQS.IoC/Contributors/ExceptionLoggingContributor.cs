﻿using Castle.Core;
using Castle.MicroKernel;
using IQ.CQS.Interceptors;

namespace IQ.CQS.IoC.Contributors
{
	internal class ExceptionLoggingContributor : CQSInterceptorContributor<LogPerformanceMetricsInterceptor>
	{
		public ExceptionLoggingContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
			: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
		}

		public override InterceptorUsageOptions HandlerTypesToApplyTo => InterceptorUsageOptions.AllHandlers;

		protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
		{
			return true;
		}
	}
}
