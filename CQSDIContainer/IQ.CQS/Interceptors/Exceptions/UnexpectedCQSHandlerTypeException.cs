using System;
using IQ.CQS.Interceptors.Enums;

namespace IQ.CQS.Interceptors.Exceptions
{
	public class UnexpectedCQSHandlerTypeException : Exception
	{
		public UnexpectedCQSHandlerTypeException(InvocationTypes offendingType, InvocationTypes expectedTypes)
			: base($"Received {offendingType}, which is unexpected!!  Expected the following: {expectedTypes}")
		{
			
		}
	}
}
