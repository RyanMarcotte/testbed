using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
	internal abstract class CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations : AutoDataAttribute
	{
		private readonly Type _cqsInterceptorCustomizationType;
		private readonly bool? _invocationCompletesSuccessfully;

		protected CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations(Type cqsInterceptorCustomizationType)
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

		protected CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully)
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
						((dynamic)interceptorFactoryInstance).CreateInterceptorWithComponentModelSet(this.Fixture, ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType))),
						CQSInvocationCustomization.BuildInvocation(false, handlerType),
					}.Concat(data.Skip(3)).ToArray();
				}

				if (_invocationCompletesSuccessfully == null || _invocationCompletesSuccessfully.Value)
				{
					yield return new object[]
					{
						((dynamic)interceptorFactoryInstance).CreateInterceptorWithComponentModelSet(this.Fixture, ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType))),
						CQSInvocationCustomization.BuildInvocation(true, handlerType)
					}.Concat(data.Skip(3)).ToArray();
				}
			}
		}
	}
}
