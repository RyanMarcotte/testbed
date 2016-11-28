using System;
using Castle.Core;
using IQ.CQS.Interceptors.TransactionScopes.Interfaces;
using IQ.Platform.Framework.Common;

namespace IQ.CQS.Interceptors
{
	/// <summary>
	/// 
	/// </summary>
	public class TransactionScopeInterceptor : CQSInterceptorWithExceptionHandling
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScopeInterceptor"/> class.
		/// </summary>
		/// <param name="transactionScopeManager">The transaction scope manager.</param>
		public TransactionScopeInterceptor(IManageTransactionScopesForCQSHandlers transactionScopeManager)
		{
			if (transactionScopeManager == null)
				throw new ArgumentNullException(nameof(transactionScopeManager));

			TransactionScopeManager = transactionScopeManager;
		}

		/// <summary>
		/// Gets the transaction scope manager.
		/// </summary>
		public IManageTransactionScopesForCQSHandlers TransactionScopeManager { get; }

		/// <summary>
		/// Called just before beginning handler invocation.  Use for setup.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			TransactionScopeManager.OpenTransactionScopeForInvocationInstance(invocationInstance);
		}

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous query handler invocation.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected override void OnReceiveReturnValueFromQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(invocationInstance, true);
		}

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous query handler invocation.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected override void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(invocationInstance, true);
		}

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous command handler invocation that does not return any value.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void OnReceiveReturnValueFromCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			CompleteTransaction(invocationInstance, true);
		}

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an synchronous command handler that returns a result.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(invocationInstance, returnValue.IsSuccessfull);
		}

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			CompleteTransaction(invocationInstance, true);
		}

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler that returns a result.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(invocationInstance, returnValue.IsSuccessfull);
		}

		/// <summary>
		/// Always called just before returning control to the caller.  Use for teardown.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
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
