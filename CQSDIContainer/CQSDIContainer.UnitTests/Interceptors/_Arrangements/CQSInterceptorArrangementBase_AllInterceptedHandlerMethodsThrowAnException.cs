using System;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// 
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
	{
		protected CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException(Type interceptorCustomizationType)
			: base(interceptorCustomizationType, false)
		{

		}
	}
}
