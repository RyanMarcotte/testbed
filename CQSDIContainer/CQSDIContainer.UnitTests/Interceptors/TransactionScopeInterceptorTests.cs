using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.TransactionScopes.Interfaces;
using CQSDIContainer.UnitTests.Arrangements;
using CQSDIContainer.UnitTests.Customizations.Utilities;
using CQSDIContainer.UnitTests.TestUtilities;
using FakeItEasy;
using Ploeh.AutoFixture;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="TransactionScopeInterceptor"/> class.
	/// </summary>
	public class TransactionScopeInterceptorTests
	{
		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(true)]
		public void OpensThenCompletesThenDisposesATransactionScopeForSuccessfulInvocation(TransactionScopeInterceptor sut, IInvocation invocation, ComponentModel componentModel, IManageTransactionScopesForCQSHandlers transactionScopeManager)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);

			A.CallTo(() => transactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened()
				.Then(A.CallTo(() => transactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened())
				.Then(A.CallTo(() => transactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened());
		}

		[Theory]
		[CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(false)]
		public void OpensThenDisposesATransactionScopeForInvocationsThatThrowExceptions(TransactionScopeInterceptor sut, IInvocation invocation, ComponentModel componentModel, IManageTransactionScopesForCQSHandlers transactionScopeManager)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);

			A.CallTo(() => transactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened()
				.Then(A.CallTo(() => transactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened());

			A.CallTo(() => transactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustNotHaveHappened();
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorWithExceptionHandlingArrangement : CQSInterceptorWithExceptionHandlingArrangementBase
		{
			public CQSInterceptorWithExceptionHandlingArrangement(bool invocationCompletesSuccessfully, CQSHandlerType methodType)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), invocationCompletesSuccessfully, methodType)
			{

			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement : CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase
		{
			public CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization))
			{

			}

			public CQSInterceptorWithExceptionHandlingAllConfigurationsArrangement(bool invocationCompletesSuccessfully)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), invocationCompletesSuccessfully)
			{

			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorWithExceptionHandlingCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<TransactionScopeInterceptor>
		{
			public override TransactionScopeInterceptor CreateInterceptor(IFixture fixture)
			{
				var transactionScopeManager = A.Fake<IManageTransactionScopesForCQSHandlers>();
				A.CallTo(() => transactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
				A.CallTo(() => transactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
				A.CallTo(() => transactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();

				fixture?.Register(() => transactionScopeManager);

				return new TransactionScopeInterceptor(transactionScopeManager);
			}
		}

		#endregion

		#region Customizations

		private class TransactionScopeInterceptorCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				var transactionScopeManager = A.Fake<IManageTransactionScopesForCQSHandlers>();
				A.CallTo(() => transactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
				A.CallTo(() => transactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
				A.CallTo(() => transactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
				
				fixture.Register(() => new TransactionScopeInterceptor(transactionScopeManager));
			}
		}

		#endregion
	}
}
