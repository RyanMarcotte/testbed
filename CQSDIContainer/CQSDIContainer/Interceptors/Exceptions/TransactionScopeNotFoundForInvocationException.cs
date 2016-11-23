using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Interceptors.Exceptions
{
	public class TransactionScopeNotFoundForInvocationException : Exception
	{
		public TransactionScopeNotFoundForInvocationException(InvocationInstance invocationInstance)
			: base($"A transaction scope for the specified invocation ({invocationInstance.ComponentModelImplementationType}.{invocationInstance.MethodName}) could not be found!!")
		{
			
		}
	}
}
