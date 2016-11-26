using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Interceptors._Arrangements.Utilities;
using CQSDIContainer.UnitTests._TestUtilities;
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
		private readonly CQSHandlerType _handlerType;

		/// <summary>
		/// Initializes a new instance of the <see cref="CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler"/> class.
		/// </summary>
		/// <param name="cqsInterceptorCustomizationType">The customization for the type of interceptor being tested.</param>
		/// <param name="invocationCompletesSuccessfully">Indicates if an invocation completes successfully.  If not, an <see cref="InvocationFailedException"/> is thrown.</param>
		/// <param name="handlerType">The CQS handler type.</param>
		/// <param name="customizations">A set of additional customizations to apply.</param>
		protected CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler(Type cqsInterceptorCustomizationType, bool invocationCompletesSuccessfully, CQSHandlerType handlerType, params ICustomization[] customizations)
			: base(new Fixture()
				.Customize(new AutoFakeItEasyCustomization())
				.Customize(new CQSInvocationCustomization(invocationCompletesSuccessfully, handlerType))
				.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType)))
				.Customize(CQSInterceptorArrangementUtility.CreateCQSInterceptorCustomizationInstance(cqsInterceptorCustomizationType))
				.CustomizeWithMany(customizations))
		{
			_handlerType = handlerType;
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

			yield return data.Take(2).Concat(AddAdditionalParametersBasedOnCQSHandlerType(data.Skip(2), _handlerType)).ToArray();
		}

		/// <summary>
		/// Generate a collection of additional parameters for unit tests (does not include the interceptor instance under test or the fake invocation instance).
		/// </summary>
		/// <param name="additionalParameters">The original collection of additional parameters.</param>
		/// <param name="handlerType">The CQS handler type.</param>
		/// <returns></returns>
		protected virtual IEnumerable<object> AddAdditionalParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
		{
			return additionalParameters;
		}
	}
}
