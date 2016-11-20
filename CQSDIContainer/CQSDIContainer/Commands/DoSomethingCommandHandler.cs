using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Attributes;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Commands
{
	[LogExecutionTime]
	public class DoSomethingCommandHandler : ICommandHandler<DoSomethingCommand>
	{
		public void Handle(DoSomethingCommand command)
		{
			for (int n = 0; n < command.Iterations; ++n)
				Console.WriteLine(n);
		}
	}
}
