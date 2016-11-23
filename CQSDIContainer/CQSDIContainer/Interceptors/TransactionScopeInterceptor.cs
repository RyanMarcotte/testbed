﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors.TransactionScopes.Interfaces;
using IQ.Platform.Framework.Common;

namespace CQSDIContainer.Interceptors
{
	public class TransactionScopeInterceptor : CQSInterceptorWithExceptionHandling
	{
		private readonly IManageTransactionScopesForCQSHandlers _transactionScopeManager;

		public TransactionScopeInterceptor(IManageTransactionScopesForCQSHandlers transactionScopeManager)
		{
			if (transactionScopeManager == null)
				throw new ArgumentNullException(nameof(transactionScopeManager));

			_transactionScopeManager = transactionScopeManager;
		}

		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			_transactionScopeManager.OpenTransactionScopeForInvocationInstance(invocationInstance);
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
			_transactionScopeManager.DisposeTransactionScopeForInvocationInstance(invocationInstance);
		}

		#region Internals

		private void CompleteTransaction(InvocationInstance invocationInstance, bool isSuccessful)
		{
			if (!isSuccessful)
				return;

			_transactionScopeManager.CompleteTransactionScopeForInvocationInstance(invocationInstance);
		}

		#endregion
	}
}
