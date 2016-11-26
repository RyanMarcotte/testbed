using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CQSDIContainer.UnitTests.Interceptors._Arrangements.Utilities;
using CQSDIContainer.UnitTests._Customizations;
using CQSDIContainer.UnitTests._TestUtilities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;

namespace CQSDIContainer.UnitTests.Interceptors._Arrangements
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// Base class for unit test arrangements that produce test data covering all CQS handler types.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	internal abstract class CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations : AutoDataAttribute
	{
		private readonly Type _cqsInterceptorCustomizationType;
		private readonly CQSHandlerTypeSelector _handlerTypesToTest;
		private readonly bool? _invocationCompletesSuccessfully;

		/// <summary>
		/// Initializes a new instance of the <see cref="CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations"/> class.  Use for tests agnostic to the invocation behavior.
		/// </summary>
		/// <param name="cqsInterceptorCustomizationType">The customization for the type of interceptor being tested.</param>
		/// <param name="customizations">A set of additional customizations to apply.</param>
		protected CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations(Type cqsInterceptorCustomizationType, params ICustomization[] customizations)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization())
				.Customize(new ComponentModelCustomization(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(CQSHandlerType.Query_ReturnsValueType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType))
				.CustomizeWithMany(customizations))
		{
			_cqsInterceptorCustomizationType = cqsInterceptorCustomizationType;

			// we will test both successful and unsuccessful invocations
			_handlerTypesToTest = CQSHandlerTypeSelector.AllHandlers;
			_invocationCompletesSuccessfully = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations"/> class.  Configures the invocation behavior.
		/// </summary>
		/// <param name="cqsInterceptorCustomizationType">The customization for the type of interceptor being tested.</param>
		/// <param name="invocationCompletesSuccessfully">Indicates if an invocation completes successfully.  If not, an <see cref="InvocationFailedException"/> is thrown.</param>
		/// <param name="customizations">A set of additional customizations to apply.</param>
		protected CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully, params ICustomization[] customizations)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization())
				.Customize(new ComponentModelCustomization(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(CQSHandlerType.Query_ReturnsValueType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType))
				.CustomizeWithMany(customizations))
		{
			_cqsInterceptorCustomizationType = cqsInterceptorCustomizationType;

			// we will test either successful or unsuccessful invocations (not both)
			_handlerTypesToTest = CQSHandlerTypeSelector.AllHandlers;
			_invocationCompletesSuccessfully = invocationCompletesSuccessfully;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations"/> class.  Configures the invocation behavior.
		/// </summary>
		/// <param name="cqsInterceptorCustomizationType">The customization for the type of interceptor being tested.</param>
		/// <param name="handlerTypeSelector"></param>
		/// <param name="invocationCompletesSuccessfully">Indicates if an invocation completes successfully.  If not, an <see cref="InvocationFailedException"/> is thrown.</param>
		/// <param name="customizations">A set of additional customizations to apply.</param>
		protected CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations(Type cqsInterceptorCustomizationType, CQSHandlerTypeSelector handlerTypeSelector, bool invocationCompletesSuccessfully, params ICustomization[] customizations)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization())
				.Customize(new ComponentModelCustomization(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(CQSHandlerType.Query_ReturnsValueType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType))
				.CustomizeWithMany(customizations))
		{
			_cqsInterceptorCustomizationType = cqsInterceptorCustomizationType;

			// we will test either successful or unsuccessful invocations (not both)
			_handlerTypesToTest = handlerTypeSelector;
			_invocationCompletesSuccessfully = invocationCompletesSuccessfully;
		}

		/// <summary>
		/// Using the original test data, generate a collection of test data corresponding to all CQS handler types and the configured invocation behavior (run successfully, throw an exception, or either).
		/// </summary>
		/// <param name="testMethod">The method to generate test data for.</param>
		/// <returns></returns>
		public sealed override IEnumerable<object[]> GetData(MethodInfo testMethod)
		{
			var data = base.GetData(testMethod).FirstOrDefault();
			if (data == null)
				throw new InvalidOperationException("Expected at least one item in the data!!");

			var interceptorFactoryInstance = CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(_cqsInterceptorCustomizationType);
			foreach (var handlerType in CQSHandlerTypeRepository.GetHandlerTypes(_handlerTypesToTest))
			{
				if (_invocationCompletesSuccessfully == null || !_invocationCompletesSuccessfully.Value)
				{
					yield return new object[]
					{
						((dynamic)interceptorFactoryInstance).CreateInterceptorWithComponentModelSet(this.Fixture, ComponentModelCustomization.BuildComponentModel(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(handlerType))),
						CQSInvocationCustomization.BuildInvocation(false, handlerType),
					}.Concat(AddAdditionalUnitTestMethodParametersBasedOnCQSHandlerType(data.Skip(2), handlerType)).ToArray();
				}

				if (_invocationCompletesSuccessfully == null || _invocationCompletesSuccessfully.Value)
				{
					yield return new object[]
					{
						((dynamic)interceptorFactoryInstance).CreateInterceptorWithComponentModelSet(this.Fixture, ComponentModelCustomization.BuildComponentModel(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(handlerType))),
						CQSInvocationCustomization.BuildInvocation(true, handlerType)
					}.Concat(AddAdditionalUnitTestMethodParametersBasedOnCQSHandlerType(data.Skip(2), handlerType)).ToArray();
				}
			}
		}

		/// <summary>
		/// Generate a collection of additional parameters for unit tests (does not include the interceptor instance under test or the fake invocation instance).
		/// </summary>
		/// <param name="additionalParameters">The original collection of additional parameters.</param>
		/// <param name="handlerType">The CQS handler type.</param>
		/// <returns></returns>
		protected virtual IEnumerable<object> AddAdditionalUnitTestMethodParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
		{
			return additionalParameters;
		}
	}
}
