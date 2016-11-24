using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests.Arrangements.Utilities;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Interceptors;
using CQSDIContainer.UnitTests.TestUtilities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Arrangements
{
	public abstract class CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase : AutoDataAttribute
	{
		private readonly Type _cqsInterceptorCustomizationType;
		private readonly bool? _invocationCompletesSuccessfully;

		protected CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase(Type cqsInterceptorCustomizationType)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization())
				.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType)))
		{
			_cqsInterceptorCustomizationType = cqsInterceptorCustomizationType;

			// we will test both successful and unsuccessful invocations
			_invocationCompletesSuccessfully = null;
		}

		protected CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization())
				.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType)))
		{
			_cqsInterceptorCustomizationType = cqsInterceptorCustomizationType;

			// we will test either successful or unsuccessful invocations (not both)
			_invocationCompletesSuccessfully = invocationCompletesSuccessfully;
		}

		public override IEnumerable<object[]> GetData(MethodInfo testMethod)
		{
			var data = base.GetData(testMethod).FirstOrDefault();
			if (data == null)
				throw new InvalidOperationException("Expected at least one item in the data!!");

			var interceptorFactoryInstance = CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(_cqsInterceptorCustomizationType);
			foreach (var handlerType in Enum.GetValues(typeof(CQSHandlerType)).Cast<CQSHandlerType>())
			{
				if (_invocationCompletesSuccessfully == null || !_invocationCompletesSuccessfully.Value)
				{
					yield return new object[]
					{
						((dynamic)interceptorFactoryInstance).CreateInterceptor(this.Fixture, true),
						CQSInvocationCustomization.BuildInvocation(false, handlerType),
						ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType))
					}.Concat(AppendExistingParameters(data)).ToArray();
				}

				if (_invocationCompletesSuccessfully == null || _invocationCompletesSuccessfully.Value)
				{
					yield return new object[]
					{
						((dynamic)interceptorFactoryInstance).CreateInterceptor(this.Fixture, true),
						CQSInvocationCustomization.BuildInvocation(true, handlerType),
						ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType))
					}.Concat(AppendExistingParameters(data)).ToArray();
				}
			}
		}

		private static IEnumerable<object> AppendExistingParameters(IEnumerable<object> existingParameters)
		{
			return existingParameters.Skip(3);
		}
	}
}
