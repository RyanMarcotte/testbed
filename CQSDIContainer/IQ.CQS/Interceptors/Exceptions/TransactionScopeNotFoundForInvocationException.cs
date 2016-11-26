using System;

namespace IQ.CQS.Interceptors.Exceptions
{
	public class TransactionScopeNotFoundForInvocationException : Exception
	{
		public TransactionScopeNotFoundForInvocationException(InvocationInstance invocationInstance)
			: base($"A transaction scope for the specified invocation ({invocationInstance.ComponentModelImplementationType}.{invocationInstance.MethodName}) could not be found!!")
		{
			
		}
	}
}
