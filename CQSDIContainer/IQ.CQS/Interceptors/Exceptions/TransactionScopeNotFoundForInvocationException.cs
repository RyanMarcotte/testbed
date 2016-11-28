using System;

namespace IQ.CQS.Interceptors.Exceptions
{
	/// <summary>
	/// Exception thrown when a transaction scope cannot be found for an intercepted invocation.  Was it disposed prematurely?
	/// </summary>
	public class TransactionScopeNotFoundForInvocationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScopeNotFoundForInvocationException"/> class.
		/// </summary>
		/// <param name="invocationInstance">The invocation instance.</param>
		public TransactionScopeNotFoundForInvocationException(InvocationInstance invocationInstance)
			: base($"A transaction scope for the specified invocation ({invocationInstance.ComponentModelImplementationType}.{invocationInstance.MethodName}) could not be found!!")
		{
			
		}
	}
}
