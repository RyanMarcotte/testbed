using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Commands
{
	public class DoSomethingWithResultCommand
	{
		public DoSomethingWithResultCommand(int numerator, int denominator)
		{
			Numerator = numerator;
			Denominator = denominator;
		}

		public int Numerator { get; }
		public int Denominator { get; }
	}
}
