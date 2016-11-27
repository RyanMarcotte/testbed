using System;
using IQ.CQS.Interceptors.Enums;

namespace IQ.CQS.Interceptors.Exceptions
{
	/// <summary>
	/// Exception thrown when an unexpected invocation type is encountered in a code flow.
	/// </summary>
	public class UnexpectedCQSHandlerTypeException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnexpectedCQSHandlerTypeException"/>.
		/// </summary>
		/// <param name="offendingType">The invocation type received.</param>
		/// <param name="expectedTypes">The expected invocation types.</param>
		public UnexpectedCQSHandlerTypeException(InvocationTypes offendingType, InvocationTypes expectedTypes)
			: base($"Received {offendingType}, which is unexpected!!  Expected the following: {expectedTypes}")
		{
			
		}
	}
}
