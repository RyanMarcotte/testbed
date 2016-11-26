using System;
using IQ.CQS.Attributes;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Commands
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
