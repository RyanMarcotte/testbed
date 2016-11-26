using System;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// 
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_AllInterceptedHandlerMethods : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
	{
		protected CQSInterceptorArrangementBase_AllInterceptedHandlerMethods(Type cqsInterceptorCustomizationType)
			: base(cqsInterceptorCustomizationType)
		{
		}
	}
}
