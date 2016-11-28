using Castle.DynamicProxy;
using FakeItEasy;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.TransactionScopes.Interfaces;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Exceptions;
using IQ.CQS.UnitTests.Interceptors._Arrangements;
using IQ.CQS.UnitTests.Interceptors._Customizations;
using Ploeh.AutoFixture;
using Xunit;

namespace IQ.CQS.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="TransactionScopeInterceptor"/> class.
	/// </summary>
	public class TransactionScopeInterceptorTests
	{
		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.Query_ReturnsValueType)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncQuery_ReturnsValueType)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.Command)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncCommand)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.ResultCommand_Succeeds)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncResultCommand_Succeeds)]
		internal void OpensThenCompletesThenDisposesATransactionScopeForSuccessfulInvocation(TransactionScopeInterceptor sut, IInvocation invocation)
		{
			sut.Intercept(invocation);

			A.CallTo(() => sut.TransactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once)
				.Then(A.CallTo(() => sut.TransactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once))
				.Then(A.CallTo(() => sut.TransactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once));
		}

		[Theory]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.ResultCommand_Fails)]
		[InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType.AsyncResultCommand_Fails)]
		internal void OpensThenCompletesThenDisposesATransactionScopeForSuccessfulInvocationButHasFailResult(TransactionScopeInterceptor sut, IInvocation invocation)
		{
			sut.Intercept(invocation);
			VerifyOpensThenDisposesATransactionScope(sut);
		}

		[Theory]
		[AllInterceptedHandlerMethodsThrowAnExceptionArrangement]
		internal void OpensThenDisposesATransactionScopeForInvocationsThatThrowExceptions(TransactionScopeInterceptor sut, IInvocation invocation)
		{
			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			VerifyOpensThenDisposesATransactionScope(sut);
		}

		private static void VerifyOpensThenDisposesATransactionScope(TransactionScopeInterceptor sut)
		{
			A.CallTo(() => sut.TransactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once)
				.Then(A.CallTo(() => sut.TransactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once));

			A.CallTo(() => sut.TransactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private class InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement : CQSInterceptorArrangementBase_InterceptedHandlerMethodDoesNotThrowAnException
		{
			public InterceptedHandlerMethodDoesNotThrowAnExceptionArrangement(CQSHandlerType handlerType)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), handlerType)
			{
			}
		}

		private class AllInterceptedHandlerMethodsThrowAnExceptionArrangement : CQSInterceptorArrangementBase_AllInterceptedHandlerMethodsThrowAnException
		{
			public AllInterceptedHandlerMethodsThrowAnExceptionArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization))
			{
				
			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorWithExceptionHandlingCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<TransactionScopeInterceptor>
		{
			protected override void RegisterDependencies(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var transactionScopeManager = A.Fake<IManageTransactionScopesForCQSHandlers>();
					A.CallTo(() => transactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
					A.CallTo(() => transactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
					A.CallTo(() => transactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();

					return transactionScopeManager;
				});
			}

			protected override TransactionScopeInterceptor CreateInterceptor(IFixture fixture)
			{
				return new TransactionScopeInterceptor(fixture.Create<IManageTransactionScopesForCQSHandlers>());
			}
		}

		#endregion
	}
}
