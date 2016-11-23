using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Exceptions;
using CQSDIContainer.Interceptors.TransactionScopes;
using CQSDIContainer.Interceptors.TransactionScopes.Interfaces;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.TestUtilities;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors.TransactionScopes
{
	/// <summary>
	/// Unit tests for the <see cref="TransactionScopeManagerForCQSHandlers"/> class.
	/// </summary>
	public class TransactionScopeManagerForCQSHandlersTests
	{
		[Theory]
		[EmptyTransactionScopeManagerForCQSHandlersArrangement]
		[NonEmptyTransactionScopeManagerForCQSHandlersArrangement]
		public void ShouldOpenNewTransactionScopeForDifferentInvocationInstances(TransactionScopeManagerForCQSHandlers sut, InvocationInstance invocationInstance)
		{
			int originalCount = sut.NumberOfOpenTransactionScopes;
			sut.OpenTransactionScopeForInvocationInstance(invocationInstance);
			sut.NumberOfOpenTransactionScopes.Should().Be(originalCount + 1);
		}

		[Theory]
		[EmptyTransactionScopeManagerForCQSHandlersArrangement]
		[NonEmptyTransactionScopeManagerForCQSHandlersArrangement]
		public void ShouldCompleteTransactionScopeIfInvocationInstanceExists(TransactionScopeManagerForCQSHandlers sut, InvocationInstance invocationInstance)
		{
			// need to perform this setup step first
			sut.OpenTransactionScopeForInvocationInstance(invocationInstance);

			int originalCount = sut.NumberOfOpenTransactionScopes;
			Action act = () => sut.CompleteTransactionScopeForInvocationInstance(invocationInstance);
			act.ShouldNotThrow<Exception>();
			sut.NumberOfOpenTransactionScopes.Should().Be(originalCount);
		}

		[Theory]
		[EmptyTransactionScopeManagerForCQSHandlersArrangement]
		[NonEmptyTransactionScopeManagerForCQSHandlersArrangement]
		public void ShouldThrowExceptionOnCompleteTransactionScopeIfNoTransactionScopeFoundForInvocationInstance(TransactionScopeManagerForCQSHandlers sut, InvocationInstance invocationInstance)
		{
			int originalCount = sut.NumberOfOpenTransactionScopes;
			Action act = () => sut.CompleteTransactionScopeForInvocationInstance(invocationInstance);
			act.ShouldThrow<TransactionScopeNotFoundForInvocationException>();
			sut.NumberOfOpenTransactionScopes.Should().Be(originalCount);
		}

		[Theory]
		[EmptyTransactionScopeManagerForCQSHandlersArrangement]
		[NonEmptyTransactionScopeManagerForCQSHandlersArrangement]
		public void ShouldDisposeAndRemoveTransactionScopeIfInvocationInstanceExists(TransactionScopeManagerForCQSHandlers sut, InvocationInstance invocationInstance)
		{
			// need to perform this setup step first
			sut.OpenTransactionScopeForInvocationInstance(invocationInstance);

			int originalCount = sut.NumberOfOpenTransactionScopes;
			sut.DisposeTransactionScopeForInvocationInstance(invocationInstance);
			sut.NumberOfOpenTransactionScopes.Should().Be(originalCount - 1);
		}

		[Theory]
		[EmptyTransactionScopeManagerForCQSHandlersArrangement]
		[NonEmptyTransactionScopeManagerForCQSHandlersArrangement]
		public void ShouldThrowExceptionOnDisposeTransactionScopeIfNoTransactionScopeFoundForInvocationInstance(TransactionScopeManagerForCQSHandlers sut, InvocationInstance invocationInstance)
		{
			int originalCount = sut.NumberOfOpenTransactionScopes;
			Action act = () => sut.DisposeTransactionScopeForInvocationInstance(invocationInstance);
			act.ShouldThrow<TransactionScopeNotFoundForInvocationException>();
			sut.NumberOfOpenTransactionScopes.Should().Be(originalCount);
		}

		#region Arrangements

		public class EmptyTransactionScopeManagerForCQSHandlersArrangement : AutoDataAttribute
		{
			public EmptyTransactionScopeManagerForCQSHandlersArrangement()
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new TransactionScopeManagerForCQSHandlersCustomization(0))
					.Customize(new InvocationInstanceCustomization()))
			{
				
			}
		}

		public class NonEmptyTransactionScopeManagerForCQSHandlersArrangement : AutoDataAttribute
		{
			public NonEmptyTransactionScopeManagerForCQSHandlersArrangement()
				: base(new Fixture()
					.Customize(new AutoFakeItEasyCustomization())
					.Customize(new TransactionScopeManagerForCQSHandlersCustomization(5))
					.Customize(new InvocationInstanceCustomization()))
			{

			}
		}

		#endregion

		#region Customizations

		private class TransactionScopeManagerForCQSHandlersCustomization : ICustomization
		{
			private readonly int _numberOfExistingTransactionScopes;

			public TransactionScopeManagerForCQSHandlersCustomization(int numberOfExistingTransactionScopes)
			{
				_numberOfExistingTransactionScopes = numberOfExistingTransactionScopes;
			}

			public void Customize(IFixture fixture)
			{
				var sut = new TransactionScopeManagerForCQSHandlers();
				for (int n = 0; n < _numberOfExistingTransactionScopes; ++n)
					InvocationInstanceCustomization.BuildInvocationInstance(CQSInvocationCustomization.BuildInvocation(true, CQSHandlerType.Query), ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query)));

				fixture.Register(() => sut);
			}
		}

		private class InvocationInstanceCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				var invocation = CQSInvocationCustomization.BuildInvocation(true, CQSHandlerType.Query);
				var componentModel = ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query));
				
				fixture.Register(() => BuildInvocationInstance(invocation, componentModel));
			}

			public static InvocationInstance BuildInvocationInstance(IInvocation invocation, ComponentModel componentModel)
			{
				return new InvocationInstance(invocation, componentModel);
			}
		}
		#endregion
	}
}
