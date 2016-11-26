using System;
using IQ.CQS.Interceptors.Enums;

namespace IQ.CQS.Interceptors.Exceptions
{
	public class CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException : Exception
	{
		public CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException(Type offendingType, InvocationTypes invocationTypes)
			: base($"The handler class {offendingType} implements more than one handler interface ({invocationTypes})!!  Please choose exactly one to implement.")
		{
			
		}
	}
}
