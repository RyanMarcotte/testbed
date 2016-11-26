using System;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Auto-creates data for interceptor unit tests that is configured to 
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
	{
		protected CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException(Type interceptorCustomizationType)
			: base(interceptorCustomizationType, true)
		{

		}
	}
}
