using System;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Interceptors._Arrangements.Utilities;
using CQSDIContainer.UnitTests.TestUtilities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for unit test arrangements that produce test data covering a specific CQS handler type's invocation behavior.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	internal abstract class CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler : AutoDataAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler"/> class.
		/// </summary>
		/// <param name="cqsInterceptorCustomizationType">The customization for the type of interceptor being tested.</param>
		/// <param name="invocationCompletesSuccessfully">Indicates if an invocation completes successfully.  If not, an <see cref="InvocationFailedException"/> is thrown.</param>
		/// <param name="handlerType">The CQS handler type.</param>
		protected CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully, CQSHandlerType handlerType)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization(invocationCompletesSuccessfully, handlerType))
				.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType)))
		{
			
		}
	}
}
