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
using CQSDIContainer.UnitTests.TestUtilities;
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
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, false, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, true, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, false, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, true, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, false, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(false, true, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, false, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandler(true, true, InvocationMethodType.SynchronousFunction)]
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
		[CQSInterceptorAlwaysAppliesArrangement(false, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorAlwaysAppliesArrangement(true, InvocationMethodType.SynchronousAction)]
		[CQSInterceptorAlwaysAppliesArrangement(false, InvocationMethodType.SynchronousFunction)]
		[CQSInterceptorAlwaysAppliesArrangement(true, InvocationMethodType.SynchronousFunction)]
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
					.Customize(new ComponentModelCustomization(ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(methodType)))
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
					.Customize(new ComponentModelCustomization(ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(methodType)))
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
