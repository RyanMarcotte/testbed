using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;

namespace CQSDIContainer.Exceptions
{
	public class UnrecognizedCQSHandlerTypeException : Exception
	{
		public UnrecognizedCQSHandlerTypeException(ComponentModel componentModel)
			: base($"Unrecognized handler type {componentModel.Implementation}!!")
		{
			
		}
	}
}
