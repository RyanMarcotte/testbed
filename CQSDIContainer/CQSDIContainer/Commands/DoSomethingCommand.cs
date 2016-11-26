﻿namespace IQ.CQS.Lab.Commands
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
