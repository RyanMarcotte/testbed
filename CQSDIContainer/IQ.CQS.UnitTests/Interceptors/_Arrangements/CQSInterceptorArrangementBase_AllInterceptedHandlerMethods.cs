﻿using System;

namespace IQ.CQS.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for unit test arrangements that produce test data covering all CQS handler types and is agnostic to invocation behavior.
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_AllInterceptedHandlerMethods : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
	{
		protected CQSInterceptorArrangementBase_AllInterceptedHandlerMethods(Type cqsInterceptorCustomizationType)
			: base(cqsInterceptorCustomizationType)
		{
		}
	}
}
