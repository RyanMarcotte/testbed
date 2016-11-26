using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests.Arrangements;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Customizations.Utilities;
using CQSDIContainer.UnitTests.TestUtilities;
using FluentAssertions;
using IQ.Platform.Framework.Common;
using Ploeh.AutoFixture;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="CQSInterceptorWithExceptionHandling"/> abstract class.
	/// </summary>
	public class CQSInterceptorWithExceptionHandlingTests
	{
		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement]
		public void ShouldAlwaysCallOnBeginInvocationMethodOnce(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnBeginInvocationCalled.Should().Be(0);

			try
			{
				sut.SetInterceptedComponentModel(componentModel);
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat it because we expect it
			}
			
			sut.NumberOfTimesOnBeginInvocationCalled.Should().Be(1);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.Query)]
		public void ShouldCallOnReceiveReturnValueFromQueryHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.AsyncQuery)]
		public void ShouldCallOnReceiveReturnValueFromAsyncQueryHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.Command)]
		public void ShouldCallOnReceiveReturnValueFromCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.ResultCommand_Succeeds)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.ResultCommand_Fails)]
		public void ShouldCallOnReceiveReturnValueFromResultCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.AsyncCommand)]
		public void ShouldCallOnReceiveReturnValueFromAsyncCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(1);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.AsyncResultCommand_Succeeds)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.AsyncResultCommand_Fails)]
		public void ShouldCallOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationMethodOnceIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(1);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(false)]
		public void ShouldNotCallOnReceiveReturnValueFromInvocationMethodIfInvocationThrowsException(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldThrow<InvocationFailedException>();
			sut.NumberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled.Should().Be(0);
			sut.NumberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled.Should().Be(0);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement]
		public void ShouldAlwaysCallOnEndInvocationMethodOnce(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnEndInvocationCalled.Should().Be(0);

			try
			{
				sut.SetInterceptedComponentModel(componentModel);
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat it because we expect it
			}

			sut.NumberOfTimesOnEndInvocationCalled.Should().Be(1);
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(true)]
		public void ShouldNotCallOnExceptionMethodIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);
			
			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0, "Exception should not have been thrown!");
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(false)]
		public void ShouldCallOnExceptionMethodOnceIfInvocationThrowsException(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);

			act.ShouldThrow<InvocationFailedException>();
			sut.NumberOfTimesOnExceptionCalled.Should().Be(1, "Exception should have been thrown!");
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorWithExceptionHandlingArrangement : CQSInterceptorWithExceptionHandlingArrangementBase
		{
			public CQSInterceptorWithExceptionHandlingArrangement(bool invocationCompletesSuccessfully, CQSHandlerType methodType)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), invocationCompletesSuccessfully, methodType)
			{

			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement : CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase
		{
			public CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization))
			{
				
			}

			public CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(bool invocationCompletesSuccessfully)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), invocationCompletesSuccessfully)
			{
				
			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorWithExceptionHandlingCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<CQSInterceptorWithExceptionHandlingImpl>
		{
			public override CQSInterceptorWithExceptionHandlingImpl CreateInterceptor(IFixture fixture, bool isInitialized)
			{
				return new CQSInterceptorWithExceptionHandlingImpl();
			}
		}

		#endregion

		#region Implementation

		public class CQSInterceptorWithExceptionHandlingImpl : CQSInterceptorWithExceptionHandling
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
