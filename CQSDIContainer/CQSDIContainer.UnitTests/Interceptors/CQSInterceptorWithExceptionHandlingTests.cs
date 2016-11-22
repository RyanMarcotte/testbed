using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.TestUtilities;
using FakeItEasy;
using FluentAssertions;
using IQ.Platform.Framework.Common;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
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
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.ResultCommand)]
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
		[CQSInterceptorWithExceptionHandlingArrangement(true, CQSHandlerType.AsyncResultCommand)]
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
		private class CQSInterceptorWithExceptionHandlingArrangement : AutoDataAttribute
		{
			public CQSInterceptorWithExceptionHandlingArrangement(bool invocationCompletesSuccessfully, CQSHandlerType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new CQSInvocationCustomization(invocationCompletesSuccessfully, methodType))
					.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(methodType)))
					.Customize(new CQSInterceptorWithExceptionHandlingCustomization()))
			{
				
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement : AutoDataAttribute
		{
			private readonly bool? _invocationCompletesSuccessfully;

			public CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement()
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new CQSInvocationCustomization())
					.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query)))
					.Customize(new CQSInterceptorWithExceptionHandlingCustomization()))
			{
				_invocationCompletesSuccessfully = null;
			}

			public CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(bool invocationCompletesSuccessfully)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new CQSInvocationCustomization())
					.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query)))
					.Customize(new CQSInterceptorWithExceptionHandlingCustomization()))
			{
				_invocationCompletesSuccessfully = invocationCompletesSuccessfully;
			}

			public override IEnumerable<object[]> GetData(MethodInfo testMethod)
			{
				var data = base.GetData(testMethod).FirstOrDefault();
				if (data == null)
					throw new InvalidOperationException("Expected at least one item in the data!!");

				foreach (var handlerType in Enum.GetValues(typeof(CQSHandlerType)).Cast<CQSHandlerType>())
				{
					if (_invocationCompletesSuccessfully == null || !_invocationCompletesSuccessfully.Value)
					{
						yield return new object[]
						{
							new CQSInterceptorWithExceptionHandlingImpl(),
							CQSInvocationCustomization.BuildInvocation(false, handlerType),
							ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType))
						};
					}

					if (_invocationCompletesSuccessfully == null || _invocationCompletesSuccessfully.Value)
					{
						yield return new object[]
						{
							new CQSInterceptorWithExceptionHandlingImpl(),
							CQSInvocationCustomization.BuildInvocation(true, handlerType),
							ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType))
						};
					}
				}
			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorWithExceptionHandlingCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register(() => new CQSInterceptorWithExceptionHandlingImpl());
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

			protected override void OnBeginInvocation(ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnBeginInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromQueryHandlerInvocation(ComponentModel componentModel, object returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromQueryHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(ComponentModel componentModel, object returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromAsyncQueryHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromCommandHandlerInvocation(ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromCommandHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromResultCommandHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(ComponentModel componentModel, Task returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromAsyncCommandHandlerInvocationCalled);
			}

			protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
			{
				Interlocked.Increment(ref _numberOfTimesOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationCalled);
			}

			protected override void OnEndInvocation(ComponentModel componentModel)
			{
				Interlocked.Increment(ref _numberOfTimesOnEndInvocationCalled);
			}

			protected override void OnException(ComponentModel componentModel, Exception ex)
			{
				Interlocked.Increment(ref _numberOfTimesOnExceptionCalled);
			}
		}

		#endregion
    }
}
