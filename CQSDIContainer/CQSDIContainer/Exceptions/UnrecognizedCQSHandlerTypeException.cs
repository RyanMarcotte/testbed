using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using CQSDIContainer.Infrastructure;
using CQSDIContainer.Utilities;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Exceptions
{
	public class UnrecognizedCQSHandlerTypeException : Exception
	{
		public UnrecognizedCQSHandlerTypeException(ComponentModel componentModel)
			: base($"{componentModel.Implementation} does not implement any recognized handler interfaces!!  The supported handler interfaces are {string.Join(", ", CQSHandlerTypeCheckingUtility.SupportedHandlerTypes.Select(x => x))}.")
		{
			
		}
	}
}
