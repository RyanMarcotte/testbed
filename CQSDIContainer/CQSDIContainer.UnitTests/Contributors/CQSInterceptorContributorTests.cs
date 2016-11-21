using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using CQSDIContainer.Contributors;
using CQSDIContainer.Contributors.Interfaces;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Attributes;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.TestUtilities;
using FakeItEasy;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace CQSDIContainer.UnitTests.Contributors
{
	public class CQSInterceptorContributorTests
	{
		[Theory] // should not apply interceptor...
		#region ... if handler is wrong type (HandlerTypesToApplyTo does not include the handler type)
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.Query, InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.Command, InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.Query, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.Command, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.QueryHandlersOnly)]
		#endregion
		#region # ... if handler type has indicated that no interceptor should be applied (ShouldApplyInterceptor returns false)
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.Query, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.Query, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.Command, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.Command, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if we're applying an interceptor to a nested handler and the interceptor does not support it
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.Query, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.Query, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.Command, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.Command, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.AllHandlers)]
		#endregion
		public void ShouldNotApplyInterceptor(ICQSInterceptorContributor sut, IKernel kernel, ComponentModel model)
		{
			model.HasInterceptors.Should().BeFalse();
			sut.ProcessModel(kernel, model);
			model.HasInterceptors.Should().BeFalse();
		}

		// TODO: fix this up
		[Theory] // should apply interceptor...
		#region ... if handler is the correct type and we're intercepting a non-nested handler
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.Query, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.Command, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.Query, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.Command, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if handler is the correct type, we're intercepting a nested handler, and the intercept supports intercepting nested handlers
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.Query, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.Command, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.Query, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.AsyncQuery, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.Command, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.AsyncCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.ResultCommand, InterceptorUsageOptions.AllHandlers)]
		[CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType.AsyncResultCommand, InterceptorUsageOptions.AllHandlers)]
		#endregion
		public void ShouldApplyInterceptor(ICQSInterceptorContributor sut, IKernel kernel, ComponentModel model)
		{
			model.HasInterceptors.Should().BeFalse();
			sut.ProcessModel(kernel, model);
			model.HasInterceptors.Should().BeTrue();
		}

		#region Arrangements

		private enum HandlerType
		{
			Query,
			AsyncQuery,
			Command,
			AsyncCommand,
			ResultCommand,
			AsyncResultCommand
		}

		private static Type GetTypeForComponentModel(HandlerType handlerType)
		{
			switch (handlerType)
			{
				case HandlerType.Query:
					return ComponentModelFactory.GetQueryHandlerComponentModelTypeFromMethodType(InvocationMethodType.SynchronousFunction);
					
				case HandlerType.AsyncQuery:
					return ComponentModelFactory.GetQueryHandlerComponentModelTypeFromMethodType(InvocationMethodType.AsynchronousFunction);

				case HandlerType.Command:
					return ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(InvocationMethodType.SynchronousAction);
					
				case HandlerType.AsyncCommand:
					return ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(InvocationMethodType.AsynchronousAction);

				case HandlerType.ResultCommand:
					return ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(InvocationMethodType.SynchronousFunction);

				case HandlerType.AsyncResultCommand:
					return ComponentModelFactory.GetCommandHandlerComponentModelTypeFromMethodType(InvocationMethodType.AsynchronousFunction);

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		private abstract class CQSInterceptorContributorArrangement : AutoDataAttribute
		{
			protected CQSInterceptorContributorArrangement(HandlerType handlerType, bool isApplyingInterceptorThatCanInterceptNestedHandlers, bool isHandlerNested, InterceptorUsageOptions usageOptions, bool shouldApplyInterceptor)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new ComponentModelCustomization(GetTypeForComponentModel(handlerType)))
					.Customize(new CQSInterceptorCustomization(isApplyingInterceptorThatCanInterceptNestedHandlers, isHandlerNested, usageOptions, shouldApplyInterceptor))
					.Customize(new KernelCustomization()))
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorContributorWithWrongHandlerTypeArrangement : CQSInterceptorContributorArrangement
		{
			private static readonly IFixture _randomizerFixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

			public CQSInterceptorContributorWithWrongHandlerTypeArrangement(HandlerType handlerType, InterceptorUsageOptions usageOptions)
				: base(handlerType, _randomizerFixture.Create<bool>(), _randomizerFixture.Create<bool>(), usageOptions, _randomizerFixture.Create<bool>())
			{
			}
		}

		private abstract class CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningTrueArrangement : CQSInterceptorContributorArrangement
		{
			protected CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningTrueArrangement(HandlerType handlerType, bool isApplyingInterceptorThatCanInterceptNestedHandlers, bool isHandlerNested, InterceptorUsageOptions usageOptions)
				: base(handlerType, isApplyingInterceptorThatCanInterceptNestedHandlers, isHandlerNested, usageOptions, true)
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement : CQSInterceptorContributorArrangement
		{
			private static readonly IFixture _randomizerFixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

			public CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(HandlerType handlerType, InterceptorUsageOptions usageOptions)
				: base(handlerType, _randomizerFixture.Create<bool>(), _randomizerFixture.Create<bool>(), usageOptions, false)
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement : CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningTrueArrangement
		{
			public CQSInterceptorContributorApplyingInterceptorNotSupportingNestedHandlerInterceptionToNestedHandlerArrangement(HandlerType handlerType, InterceptorUsageOptions usageOptions)
				: base(handlerType, false, true, usageOptions)
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement : CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningTrueArrangement
		{
			public CQSInterceptorContributorApplyingInterceptorThatDoesNotSupportNestedHandlersToNestedHandlerArrangement(HandlerType handlerType, InterceptorUsageOptions usageOptions)
				: base(handlerType, false, true, usageOptions)
			{
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement : CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningTrueArrangement
		{
			public CQSInterceptorContributorApplyingInterceptorThatSupportsNestedHandlerInterceptionsToNestedHandlerArrangement(HandlerType handlerType, InterceptorUsageOptions usageOptions)
				: base(handlerType, true, true, usageOptions)
			{
			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorCustomization : ICustomization
		{
			private readonly bool _isApplyingInterceptorThatCanInterceptNestedHandlers;
			private readonly bool _isHandlerNested;
			private readonly InterceptorUsageOptions _usageOptions;
			private readonly bool _shouldApplyInterceptor;

			public CQSInterceptorCustomization(bool isApplyingInterceptorThatCanInterceptNestedHandlers, bool isHandlerNested, InterceptorUsageOptions usageOptions, bool shouldApplyInterceptor)
			{
				_isApplyingInterceptorThatCanInterceptNestedHandlers = isApplyingInterceptorThatCanInterceptNestedHandlers;
				_isHandlerNested = isHandlerNested;
				_usageOptions = usageOptions;
				_shouldApplyInterceptor = shouldApplyInterceptor;
			}

			public void Customize(IFixture fixture)
			{
				if (_isApplyingInterceptorThatCanInterceptNestedHandlers)
					fixture.Register<ICQSInterceptorContributor>(() => new CQSInterceptorContributorImpl<CQSInterceptorThatAlsoAppliesToNestedHandlersImpl>(_isHandlerNested, _usageOptions, _shouldApplyInterceptor));
				else
					fixture.Register<ICQSInterceptorContributor>(() => new CQSInterceptorContributorImpl<CQSInterceptorImpl>(_isHandlerNested, _usageOptions, _shouldApplyInterceptor));
			}
		}

		private class KernelCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register(A.Fake<IKernel>);
			}
		}

		#endregion

		#region Implementation

		private class CQSInterceptorContributorImpl<TInterceptorType> : CQSInterceptorContributor<TInterceptorType> where TInterceptorType : CQSInterceptor
		{
			private readonly bool _shouldApplyInterceptor;

			public CQSInterceptorContributorImpl(bool isContributingToComponentModelConstructionForNestedCQSHandlers, InterceptorUsageOptions usageOptions, bool shouldApplyInterceptor)
				: base(isContributingToComponentModelConstructionForNestedCQSHandlers)
			{
				HandlerTypesToApplyTo = usageOptions;
				_shouldApplyInterceptor = shouldApplyInterceptor;
			}

			protected override InterceptorUsageOptions HandlerTypesToApplyTo { get; }

			protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
			{
				return _shouldApplyInterceptor;
			}
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		private class CQSInterceptorImpl : CQSInterceptor
		{
			protected override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
			{
				throw new NotImplementedException();
			}

			protected override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
			{
				throw new NotImplementedException();
			}
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		[ApplyToNestedHandlers]
		private class CQSInterceptorThatAlsoAppliesToNestedHandlersImpl : CQSInterceptor
		{
			protected override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
			{
				throw new NotImplementedException();
			}

			protected override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
