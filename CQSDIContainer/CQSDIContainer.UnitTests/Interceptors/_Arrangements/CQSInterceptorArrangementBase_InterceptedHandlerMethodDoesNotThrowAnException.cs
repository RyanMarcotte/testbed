using System;
using CQSDIContainer.UnitTests._TestUtilities;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for unit test arrangements that produce test data covering a specific CQS handler type with invocations that do not throw exceptions.
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_InterceptedHandlerMethodDoesNotThrowAnException : CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler
	{
		protected CQSInterceptorArrangementBase_InterceptedHandlerMethodDoesNotThrowAnException(Type interceptorCustomizationType, CQSHandlerType handlerType)
			: base(interceptorCustomizationType, true, handlerType)
		{

		}
	}
}
