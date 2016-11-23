using System;
using System.Linq;
using Castle.Core;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Interceptors.Exceptions
{
	public class UnrecognizedCQSHandlerTypeException : Exception
	{
		public UnrecognizedCQSHandlerTypeException(ComponentModel componentModel)
			: base($"{componentModel.Implementation} does not implement any recognized handler interfaces!!  The supported handler interfaces are {string.Join(", ", CQSHandlerTypeCheckingUtility.SupportedHandlerTypes.Select(x => x))}.")
		{
			
		}
	}
}
