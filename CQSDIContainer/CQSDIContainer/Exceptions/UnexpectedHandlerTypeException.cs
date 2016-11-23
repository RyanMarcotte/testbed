using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Infrastructure;

namespace CQSDIContainer.Exceptions
{
	public class UnexpectedHandlerTypeException : Exception
	{
		public UnexpectedHandlerTypeException(InvocationTypes offendingType, InvocationTypes expectedTypes)
			: base($"Received {offendingType}, which is unexpected!!  Expected the following: {expectedTypes}")
		{
			
		}
	}
}
