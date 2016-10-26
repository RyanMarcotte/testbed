using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Commands
{
	public class DoNothingAndDoSomethingCommand : ICommand
	{
		public DoNothingAndDoSomethingCommand(int numberOfIterations)
		{
			NumberOfIterations = numberOfIterations;
		}

		public int NumberOfIterations { get; }
	}
}
