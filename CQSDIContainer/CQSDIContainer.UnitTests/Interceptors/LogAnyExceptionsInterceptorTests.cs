using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;
using CQSDIContainer.UnitTests.Interceptors._Arrangements;
using CQSDIContainer.UnitTests.Interceptors._Customizations;
using CQSDIContainer.UnitTests._Customizations;
using FakeItEasy;
using Ploeh.AutoFixture;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="LogAnyExceptionsInterceptor"/> class.
	/// </summary>
	public class LogAnyExceptionsInterceptorTests
	{
		[Theory]
		[AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement]
		public void DoesNotCallExceptionLoggerIfNoExceptionWasThrownByInterceptedMethod(LogAnyExceptionsInterceptor sut, IInvocation invocation)
		{
			sut.Intercept(invocation);
			A.CallTo(() => sut.ExceptionLogger.LogException(A<Exception>._)).MustNotHaveHappened();
		}

		[Theory]
		[AllInterceptedHandlerMethodsThrowAnExceptionArrangement]
		public void CallsExceptionLoggerIfAnExceptionWasThrownByInterceptedMethod(LogAnyExceptionsInterceptor sut, IInvocation invocation)
		{
			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			A.CallTo(() => sut.ExceptionLogger.LogException(A<Exception>._)).MustHaveHappened(Repeated.Exactly.Once);
		}

		#region Arrangements

		private class AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException
		{
			public AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement()
				: base(typeof(LogAnyExceptionsInterceptorCustomization))
			{
			}
		}

		private class AllInterceptedHandlerMethodsThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException
		{
			public AllInterceptedHandlerMethodsThrowAnExceptionArrangement()
				: base(typeof(LogAnyExceptionsInterceptorCustomization))
			{
			}
		}

		#endregion

		#region Customizations

		private class LogAnyExceptionsInterceptorCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<LogAnyExceptionsInterceptor>
		{
			protected override void RegisterDependencies(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var exceptionLogger = A.Fake<ILogExceptionsFromCQSHandlers>();
					A.CallTo(() => exceptionLogger.LogException(A<Exception>._)).DoesNothing();

					return exceptionLogger;
				});
			}

			protected override LogAnyExceptionsInterceptor CreateInterceptor(IFixture fixture)
			{
				return new LogAnyExceptionsInterceptor(fixture.Create<ILogExceptionsFromCQSHandlers>());
			}
		}

		#endregion
	}
}
