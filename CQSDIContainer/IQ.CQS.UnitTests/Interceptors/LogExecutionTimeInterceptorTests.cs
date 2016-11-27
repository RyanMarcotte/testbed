using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using FakeItEasy;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.PerformanceMetricsLogging.Interfaces;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Exceptions;
using IQ.CQS.UnitTests.Framework.Utilities;
using IQ.CQS.UnitTests.Interceptors._Arrangements;
using IQ.CQS.UnitTests.Interceptors._Customizations;
using Ploeh.AutoFixture;
using Xunit;

namespace IQ.CQS.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="LogPerformanceMetricsInterceptor"/> class.
	/// </summary>
	public class LogExecutionTimeInterceptorTests
	{
		[Theory]
		[AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement]
		internal void CallsExecutionTimeLoggerIfNoExceptionWasThrownByInterceptedMethod(LogPerformanceMetricsInterceptor sut, IInvocation invocation, Type handlerType)
		{
			sut.Intercept(invocation);
			A.CallTo(() => sut.ExecutionTimeLogger.LogPerformanceMetrics(handlerType, A<object>._, A<TimeSpan>._, A<TimeSpan>._)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Theory]
		[AllInterceptedHandlerMethodsThrowAnExceptionArrangement]
		internal void CallsExecutionTimeLoggerIfAnExceptionWasThrownByInterceptedMethod(LogPerformanceMetricsInterceptor sut, IInvocation invocation, Type handlerType)
		{
			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			A.CallTo(() => sut.ExecutionTimeLogger.LogPerformanceMetrics(handlerType, A<object>._, A<TimeSpan>._, A<TimeSpan>._)).MustHaveHappened(Repeated.Exactly.Once);
		}

		#region Arrangements

		private class AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException
		{
			public AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement()
				: base(typeof(LogExecutionTimeInterceptorCustomization))
			{
			}

			protected override IEnumerable<object> AddAdditionalUnitTestMethodParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				return new object[] { SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(handlerType) };
			}
		}

		private class AllInterceptedHandlerMethodsThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException
		{
			public AllInterceptedHandlerMethodsThrowAnExceptionArrangement()
				: base(typeof(LogExecutionTimeInterceptorCustomization))
			{
			}

			protected override IEnumerable<object> AddAdditionalUnitTestMethodParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				return new object[]
				{
					SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(handlerType)
				};
			}
		}

		#endregion

		#region Customizations

		private class LogExecutionTimeInterceptorCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<LogPerformanceMetricsInterceptor>
		{
			protected override void RegisterDependencies(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var executionTimeLogger = A.Fake<ILogPerformanceMetricsForCQSHandlers>();
					A.CallTo(() => executionTimeLogger.LogPerformanceMetrics(A<Type>._, A<object>._, A<TimeSpan>._, A<TimeSpan>._)).DoesNothing();

					return executionTimeLogger;
				});
			}

			protected override LogPerformanceMetricsInterceptor CreateInterceptor(IFixture fixture)
			{
				return new LogPerformanceMetricsInterceptor(fixture.Create<ILogPerformanceMetricsForCQSHandlers>());
			}
		}

		#endregion
	}
}
