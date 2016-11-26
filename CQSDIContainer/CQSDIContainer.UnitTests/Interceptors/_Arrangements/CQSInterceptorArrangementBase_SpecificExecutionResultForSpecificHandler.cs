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
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	internal abstract class CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler : AutoDataAttribute
	{
		protected CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully, CQSHandlerType methodType)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization(invocationCompletesSuccessfully, methodType))
				.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(methodType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType)))
		{
			
		}
	}
}
