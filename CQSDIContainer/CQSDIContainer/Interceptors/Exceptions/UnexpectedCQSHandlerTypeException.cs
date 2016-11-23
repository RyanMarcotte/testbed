using System;
using CQSDIContainer.Interceptors.Enums;

namespace CQSDIContainer.Interceptors.Exceptions
{
	public class UnexpectedCQSHandlerTypeException : Exception
	{
		public UnexpectedCQSHandlerTypeException(InvocationTypes offendingType, InvocationTypes expectedTypes)
			: base($"Received {offendingType}, which is unexpected!!  Expected the following: {expectedTypes}")
		{
			
		}
	}
}
