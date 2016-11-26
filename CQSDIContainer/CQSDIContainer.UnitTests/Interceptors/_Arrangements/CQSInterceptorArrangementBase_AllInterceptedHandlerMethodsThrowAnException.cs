using System;
using CQSDIContainer.UnitTests.Customizations;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for unit test arrangements that produce test data covering all CQS handler types with invocations that throw an <see cref="InvocationFailedException"/>.
	/// </summary>
	internal abstract class CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
	{
		protected CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException(Type interceptorCustomizationType)
			: base(interceptorCustomizationType, false)
		{

		}
	}
}
