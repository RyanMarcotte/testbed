using System;

namespace IQ.CQS.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for unit test arrangements that produce test data covering all CQS handler types with invocations that do not throw an exception.
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
	{
		protected CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException(Type interceptorCustomizationType)
			: base(interceptorCustomizationType, true)
		{

		}
	}
}
