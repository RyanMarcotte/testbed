namespace IQ.CQS.Interceptors.TransactionScopes.Interfaces
{
	/// <summary>
	/// Interface of an object responsible for managing transaction scopes for CQS handlers.
	/// </summary>
	public interface IManageTransactionScopesForCQSHandlers
	{
		/// <summary>
		/// Open a new transaction scope for a invocation instance.
		/// </summary>
		/// <param name="invocationInstance">The invocation instance.</param>
		void OpenTransactionScopeForInvocationInstance(InvocationInstance invocationInstance);

		/// <summary>
		/// Complete the existing transaction scope for the specified instance, committing any changes made during invocation.
		/// </summary>
		/// <param name="invocationInstance">The invocation instance.</param>
		void CompleteTransactionScopeForInvocationInstance(InvocationInstance invocationInstance);

		/// <summary>
		/// Dispose the existing transaction scope for the specified instance.
		/// </summary>
		/// <param name="invocationInstance">The invocation instance.</param>
		void DisposeTransactionScopeForInvocationInstance(InvocationInstance invocationInstance);
	}
}
