using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests.Arrangements.Utilities;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.TestUtilities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Arrangements
{
	public abstract class CQSInterceptorWithExceptionHandlingArrangementBase : AutoDataAttribute
	{
		protected CQSInterceptorWithExceptionHandlingArrangementBase(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully, CQSHandlerType methodType)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization(invocationCompletesSuccessfully, methodType))
				.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(methodType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType)))
		{

		}
	}
}
