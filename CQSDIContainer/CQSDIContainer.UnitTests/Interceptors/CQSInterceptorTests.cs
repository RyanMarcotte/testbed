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
		[CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandlerArrangement]
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
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.Query)]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.Command)]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.ResultCommand_Succeeds)]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.ResultCommand_Fails)]
		public void ShouldOnlyCallInterceptSyncMethodIfInterceptingSynchronousMethod(CQSInterceptorImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);
			sut.InterceptSyncCalled.Should().BeTrue();
			sut.InterceptAsyncCalled.Should().BeFalse();
		}

		[Theory]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.AsyncQuery)]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.AsyncCommand)]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.AsyncResultCommand_Succeeds)]
		[CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType.AsyncResultCommand_Fails)]
		public void ShouldOnlyCallInterceptAsyncMethodIfInterceptingAsynchronousMethod(CQSInterceptorImpl sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);
			sut.InterceptSyncCalled.Should().BeFalse();
			sut.InterceptAsyncCalled.Should().BeTrue();
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandlerArrangement : AutoDataAttribute
		{
			public CQSInterceptorIsInterceptingAMethodThatDoesNotBelongToCQSHandlerArrangement()
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new CQSInvocationCustomization(false, CQSHandlerType.Query)) // doesn't matter since we're overwriting the data below
					.Customize(new ComponentModelCustomization(typeof(IEnumerable<int>)))
					.Customize(new CQSInterceptorCustomization(false))) // doesn't matter since we're overwriting the data below
			{
				
			}

			public override IEnumerable<object[]> GetData(MethodInfo testMethod)
			{
				var data = base.GetData(testMethod).FirstOrDefault();
				if (data == null)
					throw new InvalidOperationException("No data received!!");

				foreach (var handlerType in Enum.GetValues(typeof(CQSHandlerType)).Cast<CQSHandlerType>())
				{
					yield return new[] { CQSInterceptorCustomization.BuildCQSInterceptor(false), CQSInvocationCustomization.BuildInvocation(false, handlerType), data[2] };
					yield return new[] { CQSInterceptorCustomization.BuildCQSInterceptor(true), CQSInvocationCustomization.BuildInvocation(false, handlerType), data[2] };
					yield return new[] { CQSInterceptorCustomization.BuildCQSInterceptor(false), CQSInvocationCustomization.BuildInvocation(true, handlerType), data[2] };
					yield return new[] { CQSInterceptorCustomization.BuildCQSInterceptor(true), CQSInvocationCustomization.BuildInvocation(true, handlerType), data[2] };
				}
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement : AutoDataAttribute
		{
			private readonly CQSHandlerType _handlerType;

			public CQSInterceptorAlwaysAppliesAndAgnosticToInvocationSuccessArrangement(CQSHandlerType handlerType)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new CQSInvocationCustomization(false, handlerType)) // doesn't matter since we're overwriting the data below
					.Customize(new ComponentModelCustomization(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(handlerType)))
					.Customize(new CQSInterceptorCustomization(true))) // interceptor applies to all handlers (nested or not)
			{
				_handlerType = handlerType;
			}

			public override IEnumerable<object[]> GetData(MethodInfo testMethod)
			{
				var data = base.GetData(testMethod).FirstOrDefault();
				if (data == null)
					throw new InvalidOperationException("No data received!!");

				yield return new[] { data[0], CQSInvocationCustomization.BuildInvocation(false, _handlerType), data[2] };
				yield return new[] { data[0], CQSInvocationCustomization.BuildInvocation(true, _handlerType), data[2] };
			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorCustomization : ICustomization
		{
			private readonly bool _applyToNestedHandlers;

			public CQSInterceptorCustomization(bool applyToNestedHandlers)
			{
				_applyToNestedHandlers = applyToNestedHandlers;
			}

			public void Customize(IFixture fixture)
			{
				fixture.Register(() => BuildCQSInterceptor(_applyToNestedHandlers));
			}

			public static CQSInterceptorImpl BuildCQSInterceptor(bool applyToNestedHandlers)
			{
				return applyToNestedHandlers ? new CQSInterceptorThatAppliesToNestedHandlersImpl() : new CQSInterceptorImpl();
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
