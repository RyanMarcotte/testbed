using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Commands
{
	public class DoNothingCommandHandler : ICommandHandler<DoNothingCommand>
	{
		public void Handle(DoNothingCommand command)
		{
			throw new NotImplementedException();
			Console.WriteLine("I did nothing");
		}
	}
}
