using System;
using Castle.Core;
using IQ.CQS.Interceptors.Enums;

namespace IQ.CQS.Interceptors.Exceptions
{
	/// <summary>
	/// Exception thrown when a component implements multiple CQS handler interfaces.
	/// </summary>
	public class CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException"/> class.
		/// </summary>
		/// <param name="componentModel">The component model type.</param>
		/// <param name="invocationTypes">The invocation types currently associated to the component model.</param>
		public CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException(ComponentModel componentModel, InvocationTypes invocationTypes)
			: base($"The handler class {componentModel.Implementation} implements more than one handler interface ({invocationTypes})!!  Please choose exactly one to implement.")
		{
			
		}
	}
}
