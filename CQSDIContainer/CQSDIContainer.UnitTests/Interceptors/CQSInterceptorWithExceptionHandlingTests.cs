using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.TestUtilities;
using FakeItEasy;
using FluentAssertions;
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
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldAlwaysCallOnBeginInvocationMethod(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
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
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldAlwaysCallOnEndInvocationMethod(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
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
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldNotCallOnExceptionMethodIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0);

			sut.SetInterceptedComponentModel(componentModel);
			Action act = () => sut.Intercept(invocation);
			
			act.ShouldNotThrow<Exception>();
			sut.NumberOfTimesOnExceptionCalled.Should().Be(0, "Exception should not have been thrown!");
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousFunction)]
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
			public CQSInterceptorWithExceptionHandlingArrangement(bool invocationCompletesSuccessfully, InvocationMethodType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new InvocationCustomization(invocationCompletesSuccessfully, methodType))
					.Customize(new ComponentModelCustomization(ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(methodType)))
					.Customize(new CQSInterceptorWithExceptionHandlingCustomization()))
			{
				
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
			public int NumberOfTimesOnBeginInvocationCalled { get; private set; }
			public int NumberOfTimesOnEndInvocationCalled { get; private set; }
			public int NumberOfTimesOnExceptionCalled { get; private set; }

			protected override void OnBeginInvocation(ComponentModel componentModel)
			{
				NumberOfTimesOnBeginInvocationCalled++;
			}

			protected override void OnEndInvocation(ComponentModel componentModel)
			{
				NumberOfTimesOnEndInvocationCalled++;
			}

			protected override void OnException(ComponentModel componentModel, Exception ex)
			{
				NumberOfTimesOnExceptionCalled++;
			}
		}

		#endregion
    }
}
