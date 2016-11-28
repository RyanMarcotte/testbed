using System;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Commands
{
	public class DoNothingCommandHandler : ICommandHandler<DoNothingCommand>
	{
		public void Handle(DoNothingCommand command)
		{
			Console.WriteLine("I did nothing");
		}
	}
}
