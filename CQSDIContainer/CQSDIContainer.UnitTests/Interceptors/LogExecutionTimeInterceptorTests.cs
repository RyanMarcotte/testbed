using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;
using CQSDIContainer.Interceptors.MetricsLogging.Interfaces;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Interceptors._Arrangements;
using CQSDIContainer.UnitTests.Interceptors._Customizations;
using CQSDIContainer.UnitTests._TestUtilities;
using FakeItEasy;
using Ploeh.AutoFixture;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="LogExecutionTimeInterceptor"/> class.
	/// </summary>
	public class LogExecutionTimeInterceptorTests
	{
		[Theory]
		[AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement]
		public void CallsExecutionTimeLoggerIfNoExceptionWasThrownByInterceptedMethod(LogExecutionTimeInterceptor sut, IInvocation invocation, Type handlerType)
		{
			sut.Intercept(invocation);
			A.CallTo(() => sut.ExecutionTimeLogger.LogExecutionTime(handlerType, A<TimeSpan>._, A<TimeSpan>._)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Theory]
		[AllInterceptedHandlerMethodsThrowAnExceptionArrangement]
		public void CallsExecutionTimeLoggerIfAnExceptionWasThrownByInterceptedMethod(LogExecutionTimeInterceptor sut, IInvocation invocation, Type handlerType)
		{
			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			A.CallTo(() => sut.ExecutionTimeLogger.LogExecutionTime(handlerType, A<TimeSpan>._, A<TimeSpan>._)).MustHaveHappened(Repeated.Exactly.Once);
		}

		#region Arrangements

		private class AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException
		{
			public AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement()
				: base(typeof(LogExecutionTimeInterceptorCustomization))
			{
			}

			protected override IEnumerable<object> AddAdditionalParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				return new object[]
				{
					SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType)
				};
			}
		}

		private class AllInterceptedHandlerMethodsThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException
		{
			public AllInterceptedHandlerMethodsThrowAnExceptionArrangement()
				: base(typeof(LogExecutionTimeInterceptorCustomization))
			{
			}

			protected override IEnumerable<object> AddAdditionalParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				return new object[]
				{
					SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType)
				};
			}
		}

		#endregion

		#region Customizations

		private class LogExecutionTimeInterceptorCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<LogExecutionTimeInterceptor>
		{
			protected override void RegisterDependencies(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var executionTimeLogger = A.Fake<ILogExecutionTimeOfCQSHandlers>();
					A.CallTo(() => executionTimeLogger.LogExecutionTime(A<Type>._, A<TimeSpan>._, A<TimeSpan>._)).DoesNothing();

					return executionTimeLogger;
				});
			}

			protected override LogExecutionTimeInterceptor CreateInterceptor(IFixture fixture)
			{
				return new LogExecutionTimeInterceptor(fixture.Create<ILogExecutionTimeOfCQSHandlers>());
			}
		}

		#endregion
	}
}
