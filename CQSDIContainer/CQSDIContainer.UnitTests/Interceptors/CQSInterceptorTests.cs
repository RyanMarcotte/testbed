using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests.Customizations;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	public class CQSInterceptorTests
	{
		[Theory]
		[CQSInterceptorArrangement(false, InvocationMethodType.Synchronous)]
		[CQSInterceptorArrangement(true, InvocationMethodType.Synchronous)]
		public void ShouldOnlyCallInterceptSyncMethodIfInterceptingSynchronousMethod(CQSInterceptorImpl sut, IInvocation invocation)
		{
			sut.Intercept(invocation);
			sut.InterceptSyncCalled.Should().BeTrue();
			sut.InterceptAsyncCalled.Should().BeFalse();
		}

		[Theory]
		[CQSInterceptorArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorArrangement(false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldOnlyCallInterceptAsyncMethodIfInterceptingAsynchronousMethod(CQSInterceptorImpl sut, IInvocation invocation)
		{
			sut.Intercept(invocation);
			sut.InterceptSyncCalled.Should().BeFalse();
			sut.InterceptAsyncCalled.Should().BeTrue();
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorArrangement : AutoDataAttribute
		{
			public CQSInterceptorArrangement(bool invocationCompletesSuccessfully, InvocationMethodType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new InvocationCustomization(invocationCompletesSuccessfully, methodType))
					.Customize(new CQSInterceptorCustomization()))
			{

			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register(() => new CQSInterceptorImpl());
			}
		}

		#endregion

		#region Implementation

		public class CQSInterceptorImpl : CQSInterceptor
		{
			protected override bool ApplyToNestedHandlers => true;
			public bool InterceptSyncCalled { get; private set; }
			public bool InterceptAsyncCalled { get; private set; }
			
			protected override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
			{
				InterceptSyncCalled = true;
			}

			protected override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
			{
				InterceptAsyncCalled = true;
			}
		}

		#endregion
	}
}
