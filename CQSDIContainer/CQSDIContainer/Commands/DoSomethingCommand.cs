using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Commands
{
	public class DoSomethingCommand : ICommand
	{
		public DoSomethingCommand(int iterations)
		{
			Iterations = iterations;
		}

		public int Iterations { get; }
	}
}
