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
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.Synchronous)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.Synchronous)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldAlwaysCallOnBeginInvocationMethod(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.OnBeginInvocationCalled.Should().BeFalse();

			try
			{
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat it because we expect it
			}
			
			sut.OnBeginInvocationCalled.Should().BeTrue();
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.Synchronous)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.Synchronous)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldAlwaysCallOnEndInvocationMethod(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			sut.OnEndInvocationCalled.Should().BeFalse();

			try
			{
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat it because we expect it
			}

			sut.OnEndInvocationCalled.Should().BeTrue();
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.Synchronous)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldNotCallOnExceptionMethodIfInvocationCompletesSuccessfully(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			Action act = () => sut.Intercept(invocation);
			act.ShouldNotThrow<InvocationFailedException>();
			sut.OnExceptionCalled.Should().BeFalse("Exception should not have been thrown!");
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.Synchronous)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorWithExceptionHandlingArrangement(false, InvocationMethodType.AsynchronousFunction)]
		public void ShouldCallOnExceptionMethodIfInvocationThrowsException(CQSInterceptorWithExceptionHandlingImpl sut, IInvocation invocation)
		{
			Action act = () => sut.Intercept(invocation);
			act.ShouldThrow<InvocationFailedException>();
			sut.OnExceptionCalled.Should().BeTrue("Exception should have been thrown!");
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorWithExceptionHandlingArrangement : AutoDataAttribute
		{
			public CQSInterceptorWithExceptionHandlingArrangement(bool invocationCompletesSuccessfully, InvocationMethodType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new InvocationCustomization(invocationCompletesSuccessfully, methodType))
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
			protected override bool ApplyToNestedHandlers => true;
			public bool OnBeginInvocationCalled { get; private set; }
			public bool OnEndInvocationCalled { get; private set; }
			public bool OnExceptionCalled { get; private set; }

			protected override void OnBeginInvocation(ComponentModel componentModel)
			{
				OnBeginInvocationCalled = true;
			}

			protected override void OnEndInvocation(ComponentModel componentModel)
			{
				OnEndInvocationCalled = true;
			}

			protected override void OnException(ComponentModel componentModel, Exception ex)
			{
				OnExceptionCalled = true;
			}
		}

		#endregion
    }
}
