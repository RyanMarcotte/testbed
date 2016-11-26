using System;
using FluentAssertions;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Exceptions;
using IQ.CQS.Interceptors.TransactionScopes;
using IQ.CQS.UnitTests.Framework.Customizations;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Utilities;
using IQ.CQS.UnitTests._Customizations;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace IQ.CQS.UnitTests.Interceptors.TransactionScopes
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

			// need this teardown step due to static backing store
			sut.DisposeAll();
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

			// need this teardown step due to static backing store
			sut.DisposeAll();
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

			// need this teardown step due to static backing store
			sut.DisposeAll();
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

			// need this teardown step due to static backing store
			sut.DisposeAll();
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

			// need this teardown step due to static backing store
			sut.DisposeAll();
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
				fixture.Register(() =>
				{
					var sut = new TransactionScopeManagerForCQSHandlers();
					for (int n = 0; n < _numberOfExistingTransactionScopes; ++n)
						InvocationInstanceCustomization.BuildInvocationInstance(CQSInvocationCustomization.BuildInvocation(true, CQSHandlerType.Command), ComponentModelCustomization.BuildComponentModel(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(CQSHandlerType.Command)));
					
					return sut;
				});
			}
		}
		
		#endregion
	}
}
