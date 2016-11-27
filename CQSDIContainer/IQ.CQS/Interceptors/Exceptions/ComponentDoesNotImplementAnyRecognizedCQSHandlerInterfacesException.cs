using System;
using System.Linq;
using Castle.Core;
using IQ.CQS.Utilities;

namespace IQ.CQS.Interceptors.Exceptions
{
	/// <summary>
	/// Exception thrown when a component does not implement any CQS handler interfaces.
	/// </summary>
	public class ComponentDoesNotImplementAnyRecognizedCQSHandlerInterfacesException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentDoesNotImplementAnyRecognizedCQSHandlerInterfacesException"/> class.
		/// </summary>
		/// <param name="componentModel">The component model.</param>
		public ComponentDoesNotImplementAnyRecognizedCQSHandlerInterfacesException(ComponentModel componentModel)
			: base($"{componentModel.Implementation} does not implement any recognized handler interfaces!!  The supported handler interfaces are {string.Join(", ", $"{CQSHandlerTypeCheckingUtility.SupportedHandlerTypes.Select(x => x)}")}")
		{
			
		}
	}
}
