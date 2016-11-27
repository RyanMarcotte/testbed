using System;
using Castle.Core;
using IQ.CQS.Interceptors.TransactionScopes.Interfaces;
using IQ.Platform.Framework.Common;

namespace IQ.CQS.Interceptors
{
	internal class TransactionScopeInterceptor : CQSInterceptorWithExceptionHandling
	{
		public TransactionScopeInterceptor(IManageTransactionScopesForCQSHandlers transactionScopeManager)
		{
			if (transactionScopeManager == null)
				throw new ArgumentNullException(nameof(transactionScopeManager));

			TransactionScopeManager = transactionScopeManager;
		}

		public IManageTransactionScopesForCQSHandlers TransactionScopeManager { get; }

		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			TransactionScopeManager.OpenTransactionScopeForInvocationInstance(invocationInstance);
		}

		protected override void OnReceiveReturnValueFromQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(invocationInstance, true);
		}

		protected override void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(invocationInstance, true);
		}

		protected override void OnReceiveReturnValueFromCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			CompleteTransaction(invocationInstance, true);
		}

		protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(invocationInstance, returnValue.IsSuccessfull);
		}

		protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			CompleteTransaction(invocationInstance, true);
		}

		protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(invocationInstance, returnValue.IsSuccessfull);
		}

		protected override void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			TransactionScopeManager.DisposeTransactionScopeForInvocationInstance(invocationInstance);
		}

		#region Internals

		private void CompleteTransaction(InvocationInstance invocationInstance, bool isSuccessful)
		{
			if (!isSuccessful)
				return;

			TransactionScopeManager.CompleteTransactionScopeForInvocationInstance(invocationInstance);
		}

		#endregion
	}
}
