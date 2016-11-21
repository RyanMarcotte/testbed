using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Attributes;
using CQSDIContainer.UnitTests.Customizations;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	public class CQSInterceptorTests : CQSInterceptorTestsBase
	{
		[Theory]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, false, InvocationMethodType.Synchronous)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, true, InvocationMethodType.Synchronous)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, false, InvocationMethodType.Synchronous)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, true, InvocationMethodType.Synchronous)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, true, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldThrowExceptionIfInterceptingAMethodNotBelongingToACQSHandler(CQSInterceptorImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			bool throwInvalidOperationException = false;
			sut.SetInterceptedComponentModel(componentModel);
			try
			{
				sut.Intercept(invocation);
			}
			catch (InvalidOperationException)
			{
				throwInvalidOperationException = true;
			}

			throwInvalidOperationException.Should().BeTrue();
		}

		[Theory]
		[CQSInterceptorAlwaysAppliesArrangement(false, InvocationMethodType.Synchronous)]
		[CQSInterceptorAlwaysAppliesArrangement(true, InvocationMethodType.Synchronous)]
		public void ShouldOnlyCallInterceptSyncMethodIfInterceptingSynchronousMethod(CQSInterceptorImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);
			sut.InterceptSyncCalled.Should().BeTrue();
			sut.InterceptAsyncCalled.Should().BeFalse();
		}

		[Theory]
		[CQSInterceptorAlwaysAppliesArrangement(false, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorAlwaysAppliesArrangement(true, InvocationMethodType.AsynchronousAction)]
		[CQSInterceptorAlwaysAppliesArrangement(false, InvocationMethodType.AsynchronousFunction)]
		[CQSInterceptorAlwaysAppliesArrangement(true, InvocationMethodType.AsynchronousFunction)]
		public void ShouldOnlyCallInterceptAsyncMethodIfInterceptingAsynchronousMethod(CQSInterceptorImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);
			sut.InterceptSyncCalled.Should().BeFalse();
			sut.InterceptAsyncCalled.Should().BeTrue();
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler : AutoDataAttribute
		{
			public CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(bool applyToNestedHandlers, bool invocationCompletesSuccessfully, InvocationMethodType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new InvocationCustomization(invocationCompletesSuccessfully, methodType))
					.Customize(new ComponentModelCustomization(typeof(StringBuilder)))
					.Customize(new CQSInterceptorCustomization(applyToNestedHandlers)))
			{
				
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorIsInterceptingNestedHandlerArrangement : AutoDataAttribute
		{
			public CQSInterceptorIsInterceptingNestedHandlerArrangement(bool invocationCompletesSuccessfully, InvocationMethodType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new InvocationCustomization(invocationCompletesSuccessfully, methodType))
					.Customize(new ComponentModelCustomization(GetComponentModelTypeFromMethodType(methodType)))
					.Customize(new CQSInterceptorCustomization(false)))
			{

			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorAlwaysAppliesArrangement : AutoDataAttribute
		{
			public CQSInterceptorAlwaysAppliesArrangement(bool invocationCompletesSuccessfully, InvocationMethodType methodType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new InvocationCustomization(invocationCompletesSuccessfully, methodType))
					.Customize(new ComponentModelCustomization(GetComponentModelTypeFromMethodType(methodType)))
					.Customize(new CQSInterceptorCustomization(true)))
			{

			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorCustomization : ICustomization
		{
			private readonly Guid _id = Guid.NewGuid();
			private readonly bool _applyToNestedHandlers;

			public CQSInterceptorCustomization(bool applyToNestedHandlers)
			{
				_applyToNestedHandlers = applyToNestedHandlers;
			}

			public void Customize(IFixture fixture)
			{
				if (_applyToNestedHandlers)
					fixture.Register<CQSInterceptorImpl>(() => new CQSInterceptorThatAppliesToNestedHandlersImpl());
				else
					fixture.Register(() => new CQSInterceptorImpl());
			}
		}

		#endregion

		#region Implementation

		public class CQSInterceptorImpl : CQSInterceptor
		{
			public bool InterceptSyncCalled { get; private set; }
			public bool InterceptAsyncCalled { get; private set; }

			protected sealed override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
			{
				InterceptSyncCalled = true;
			}

			protected sealed override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
			{
				InterceptAsyncCalled = true;
			}
		}

		[ApplyToNestedHandlers]
		public class CQSInterceptorThatAppliesToNestedHandlersImpl : CQSInterceptorImpl
		{
			
		}

		#endregion
	}
}
