using System;
using CQSDIContainer.UnitTests.TestUtilities;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// 
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_InterceptedHandlerMethodDoesNotThrowAnException : CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler
	{
		protected CQSInterceptorArrangementBase_InterceptedHandlerMethodDoesNotThrowAnException(Type interceptorCustomizationType, CQSHandlerType methodType)
			: base(interceptorCustomizationType, true, methodType)
		{

		}
	}
}
