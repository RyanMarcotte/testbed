using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.Core;
using Castle.DynamicProxy;
using FakeItEasy;
using FluentAssertions;
using IQ.CQS.Interceptors;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Exceptions;
using IQ.CQS.UnitTests.Interceptors._Arrangements;
using IQ.CQS.UnitTests.Interceptors._Customizations;
using IQ.Vanilla;
using Ploeh.AutoFixture;
using Xunit;

namespace IQ.CQS.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="CQSInterceptorWithExceptionHandling"/> abstract class.
	/// </summary>
	public class CQSInterceptorWithExceptionHandlingTests
	{
		[Theory]
		[AllInterceptedHandlerMethodsArrangement]
		internal void ShouldAlwaysCallOnBeginInvocationMethodOnce(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnBeginInvocationCalled.Should().Be(0);

			try
			{
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat it because we expect it
			}

			sut.NumberOfTimesOnBeginInvocationCalled.Should().Be(1);
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.Query_ReturnsValueType)]
		internal void ShouldCallOnReceiveReturnValueFromQueryHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.Intercept(invocation);

			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncQuery_ReturnsValueType)]
		internal void ShouldCallOnReceiveReturnValueFromAsyncQueryHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.Intercept(invocation);

			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.Command)]
		internal void ShouldCallOnReceiveReturnValueFromCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.Intercept(invocation);

			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.ResultCommand_Succeeds)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.ResultCommand_Fails)]
		internal void ShouldCallOnReceiveReturnValueFromResultCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.Intercept(invocation);

			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncCommand)]
		internal void ShouldCallOnReceiveReturnValueFromAsyncCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.Intercept(invocation);

			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncResultCommand_Succeeds)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncResultCommand_Fails)]
		internal void ShouldCallOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.Intercept(invocation);

			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(1);
		}

		[Theory]
		[AllInterceptedHandlerMethodsThrowAnExceptionArrangement]
		internal void ShouldNotCallOnReceiveReturnValueFromInvocationMethodIfInvocationThrowsException(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[AllInterceptedHandlerMethodsArrangement]
		internal void ShouldAlwaysCallOnEndInvocationMethodOnce(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnEndInvocationCalled.Should().Be(0);

			try
			{
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat the exception since we expect it
			}
			
			sut.NumberOfTimesOnEndInvocationCalled.Should().Be(1);
		}

		[Theory]
		[AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement]
		internal void ShouldNotCallOnExceptionMethodIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0);
			sut.Intercept(invocation);
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0, "Exception should not have been thrown!");
		}

		[Theory]
		[AllInterceptedHandlerMethodsThrowAnExceptionArrangement]
		internal void ShouldCallOnExceptionMethodOnceIfInvocationThrowsException(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0);
			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			sut.NumberOfTimesOnExceptionCalled.Should().Be(1, "Exception should have been thrown!");
		}

		[Theory]
		[AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement]
		internal void AnInterceptedInvocationWithMultipleInterceptorsAppliedShouldHaveTheInvocationProceedThatManyTimes(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			const int numberOfInterceptors = 10;

			// we perform some additional setup for our invocation
			// http://stackoverflow.com/a/19731759 (perform different actions depending on which invocation)
			int numInvocations = 0;
			A.CallTo(() => invocation.Proceed()).Invokes(() =>
			{
				if (numInvocations >= numberOfInterceptors - 1)
					return;

				++numInvocations;
				var nextInterceptor = new CQSInterceptorWithExceptionHandlingImpl();
				nextInterceptor.SetInterceptedComponentModel(sut.ComponentModel);
				nextInterceptor.Intercept(invocation);
			});

			A.CallTo(() => invocation.Proceed()).MustNotHaveHappened();
			sut.Intercept(invocation);
			A.CallTo(() => invocation.Proceed()).MustHaveHappened(Repeated.Exactly.Times(numberOfInterceptors));
		}

		#region Arrangements

		private class AllInterceptedHandlerMethodsArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethods
		{
			public AllInterceptedHandlerMethodsArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization))
			{
				
			}
		}

		private class AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsDoNotThrowAnException
		{
			public AllInterceptedHandlerMethodsDoNotThrowAnExceptionArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization))
			{
				
			}
		}

		private class InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement : CQSInterceptorArrangementBase_InterceptedHandlerMethodDoesNotThrowAnException
		{
			public InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType handlerType)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), handlerType)
			{

			}
		}

		private class AllInterceptedHandlerMethodsThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException
		{
			public AllInterceptedHandlerMethodsThrowAnExceptionArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization))
			{

			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorWithExceptionHandlingCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<CQSInterceptorWithExceptionHandlingImpl>
		{
			protected override void RegisterDependencies(IFixture fixture)
			{
				
			}

			protected override CQSInterceptorWithExceptionHandlingImpl CreateInterceptor(IFixture fixture)
			{
				return new CQSInterceptorWithExceptionHandlingImpl();
			}
		}

		#endregion

		#region Implementation

		internal class CQSInterceptorWithExceptionHandlingImpl : CQSInterceptorWithExceptionHandling
		{
			private int _numberOfTimesOnBeginInvocationCalled;
			private int _numberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled;
			private int _numberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled;
			private int _numberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled;
			private int _numberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled;
			private int _numberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled;
			private int _numberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled;
			private int _numberOfTimesOnEndInvocationCalled;
			private int _numberOfTimesOnExceptionCalled;

			public int NumberOfTimesOnBeginInvocationCalled => _numberOfTimesOnBeginInvocationCalled;
			public int NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled => _numberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled;
			public int NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled => _numberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled;
			public int NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled => _numberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled;
			public int NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled => _numberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled;
			public int NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled => _numberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled;
			public int NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled => _numberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled;
			public int NumberOfTimesOnEndInvocationCalled => _numberOfTimesOnEndInvocationCalled;
			public int NumberOfTimesOnExceptionCalled => _numberOfTimesOnExceptionCalled;

			protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnBeginInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled);
			}

			protected override void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnEndInvocationCalled);
			}

			protected override void OnException(InvocationInstance invocationInstance, ComponentModel componentModel, Exception ex)
			{
				Interlocked.Increment(ref _numberOfTimesOnExceptionCalled);
			}
		}

		#endregion
    }
}
