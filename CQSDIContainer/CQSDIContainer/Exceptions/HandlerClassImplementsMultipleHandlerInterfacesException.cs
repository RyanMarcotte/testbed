using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Infrastructure;

namespace CQSDIContainer.Exceptions
{
	public class HandlerClassImplementsMultipleHandlerInterfacesException : Exception
	{
		public HandlerClassImplementsMultipleHandlerInterfacesException(Type offendingType, InvocationTypes invocationTypes)
			: base($"The handler class {offendingType} implements more than one handler interface ({invocationTypes})!!  Please choose exactly one to implement.")
		{
			
		}
	}
}
