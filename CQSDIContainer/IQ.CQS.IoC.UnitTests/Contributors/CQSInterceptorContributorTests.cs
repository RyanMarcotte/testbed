using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using FluentAssertions;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Attributes;
using IQ.CQS.IoC.Contributors;
using IQ.CQS.IoC.Contributors.Interfaces;
using IQ.CQS.UnitTests.Framework.Customizations;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Utilities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace IQ.CQS.IoC.UnitTests.Contributors
{
	public class CQSInterceptorContributorTests
	{
		[Theory] // should not apply interceptor...
		#region ... if handler is wrong type (HandlerTypesToApplyTo does not include the handler type)
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithWrongHandlerTypeArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region # ... if handler type has indicated that no interceptor should be applied (ShouldApplyInterceptor returns false)
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(InterceptorUsageOptions.None)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if we're applying an interceptor to a nested handler and the interceptor does not support interception of nested handlers
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.None)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		public void ShouldNotApplyInterceptor(ICQSInterceptorContributor sut, IKernel kernel, ComponentModel model)
		{
			model.HasInterceptors.Should().BeFalse();
			sut.ProcessModel(kernel, model);
			model.HasInterceptors.Should().BeFalse();
		}

		[Theory] // should apply interceptor...
		#region ... if handler is the correct type and we're intercepting a non-nested handler
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if handler is the correct type, we're intercepting a **non-nested** handler, and the interceptor does not support intercepting nested handlers
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if handler is the correct type, we're intercepting a **nested** handler, and the intercept supports intercepting nested handlers
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		public void ShouldApplyInterceptor(ICQSInterceptorContributor sut, IKernel kernel, ComponentModel model)
		{
			model.HasInterceptors.Should().BeFalse();
			sut.ProcessModel(kernel, model);
			model.HasInterceptors.Should().BeTrue();
		}

		[Theory]
		#region ... if handler is the correct type and we're intercepting a non-nested handler
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if handler is the correct type, we're intercepting a **non-nested** handler, and the interceptor does not support intercepting nested handlers
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		#region ... if handler is the correct type, we're intercepting a **nested** handler, and the intercept supports intercepting nested handlers
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.QueryHandlersOnly)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.CommandHandlersOnly)]
		[ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions.AllHandlers)]
		#endregion
		public void ShouldOnlyApplyInterceptorOnce(ICQSInterceptorContributor sut, IKernel kernel, ComponentModel model)
		{
			sut.ProcessModel(kernel, model);
			sut.ProcessModel(kernel, model);
			model.Interceptors.Count.Should().Be(1);
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private abstract class CQSInterceptorContributorArrangement : AutoDataAttribute
		{
			protected CQSInterceptorContributorArrangement(InterceptorUsageOptions usageOptions, bool isApplyingInterceptorThatCanInterceptNestedHandlers, bool isHandlerNested, bool shouldApplyInterceptor)
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new ComponentModelCustomization()) // we're going to overwrite that parameter, but need to generate one first
					.Customize(new CQSInterceptorCustomization(isApplyingInterceptorThatCanInterceptNestedHandlers, isHandlerNested, usageOptions, shouldApplyInterceptor))
					.Customize(new NullKernelCustomization()))
			{
			}

			public sealed override IEnumerable<object[]> GetData(MethodInfo testMethod)
			{
				var data = base.GetData(testMethod).FirstOrDefault();
				if (data == null)
					throw new InvalidOperationException("Wrong test data format!!");

				// we overwrite the ComponentModel parameter that will be passed to the unit test method
				var cqsContributor = (ICQSInterceptorContributor)data[0];
				foreach (var type in HandlerTypeLookup[cqsContributor.HandlerTypesToApplyTo])
					yield return new[] { data[0], data[1], ComponentModelCustomization.BuildComponentModel(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(type)) };
			}

			protected abstract IReadOnlyDictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>> HandlerTypeLookup { get; }
		}

		private class CQSInterceptorContributorWithWrongHandlerTypeArrangement : CQSInterceptorContributorArrangement
		{
			private static readonly IFixture _randomizerFixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

			public CQSInterceptorContributorWithWrongHandlerTypeArrangement(InterceptorUsageOptions usageOptions)
				: base(usageOptions, _randomizerFixture.Create<bool>(), _randomizerFixture.Create<bool>(), _randomizerFixture.Create<bool>())
			{
			}

			// match a usage option against all invalid handler types
			protected override IReadOnlyDictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>> HandlerTypeLookup => new Dictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>>
				{
					{ InterceptorUsageOptions.None, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllHandlers) },
					{ InterceptorUsageOptions.QueryHandlersOnly, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllCommandHandlers) },
					{ InterceptorUsageOptions.CommandHandlersOnly, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllQueryHandlers) },
					{ InterceptorUsageOptions.AllHandlers, Enumerable.Empty<CQSHandlerType>() }
				};
		}

		private class CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement : CQSInterceptorContributorArrangement
		{
			private static readonly IFixture _randomizerFixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

			public CQSInterceptorContributorWithShouldApplyInterceptorMethodReturningFalseArrangement(InterceptorUsageOptions usageOptions)
				: base(usageOptions, _randomizerFixture.Create<bool>(), _randomizerFixture.Create<bool>(), false)
			{

			}

			// match a usage option against the correct handler types
			protected override IReadOnlyDictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>> HandlerTypeLookup => new Dictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>>
				{
					{ InterceptorUsageOptions.None, Enumerable.Empty<CQSHandlerType>() },
					{ InterceptorUsageOptions.QueryHandlersOnly, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllCommandHandlers) },
					{ InterceptorUsageOptions.CommandHandlersOnly, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllQueryHandlers) },
					{ InterceptorUsageOptions.AllHandlers, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllHandlers) }
				};
		}

		private abstract class CQSInterceptorContributorWithCorrectHandlerTypeAndHaveApplyInterceptorMethodReturningTrueArrangement : CQSInterceptorContributorArrangement
		{
			protected CQSInterceptorContributorWithCorrectHandlerTypeAndHaveApplyInterceptorMethodReturningTrueArrangement(InterceptorUsageOptions usageOptions, bool isApplyingInterceptorThatCanInterceptNestedHandlers, bool isHandlerNested)
				: base(usageOptions, isApplyingInterceptorThatCanInterceptNestedHandlers, isHandlerNested, true)
			{
				
			}

			protected sealed override IReadOnlyDictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>> HandlerTypeLookup => new Dictionary<InterceptorUsageOptions, IEnumerable<CQSHandlerType>>
				{
					{ InterceptorUsageOptions.None, Enumerable.Empty<CQSHandlerType>() },
					{ InterceptorUsageOptions.QueryHandlersOnly, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllQueryHandlers) },
					{ InterceptorUsageOptions.CommandHandlersOnly, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllCommandHandlers) },
					{ InterceptorUsageOptions.AllHandlers, CQSHandlerTypeRepository.GetHandlerTypes(CQSHandlerTypeSelector.AllHandlers) }
				};
		}

		private class ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement : CQSInterceptorContributorWithCorrectHandlerTypeAndHaveApplyInterceptorMethodReturningTrueArrangement
		{
			public ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions usageOptions)
				: base(usageOptions, false, true)
			{
			}
		}

		private class ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement : CQSInterceptorContributorWithCorrectHandlerTypeAndHaveApplyInterceptorMethodReturningTrueArrangement
		{
			public ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesNotSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions usageOptions)
				: base(usageOptions, false, false)
			{
			}
		}

		private class ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement : CQSInterceptorContributorWithCorrectHandlerTypeAndHaveApplyInterceptorMethodReturningTrueArrangement
		{
			public ApplyingAnInterceptorToNonNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions usageOptions)
				: base(usageOptions, true, false)
			{
			}
		}

		private class ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement : CQSInterceptorContributorWithCorrectHandlerTypeAndHaveApplyInterceptorMethodReturningTrueArrangement
		{
			public ApplyingAnInterceptorToNestedHandlerAndInterceptorDoesSupportInterceptionOfNestedHandlersArrangement(InterceptorUsageOptions usageOptions)
				: base(usageOptions, true, true)
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

			public override InterceptorUsageOptions HandlerTypesToApplyTo { get; }

			protected override bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model)
			{
				return _shouldApplyInterceptor;
			}
		}

		#region Interceptor implementations

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

		#endregion
	}
}
