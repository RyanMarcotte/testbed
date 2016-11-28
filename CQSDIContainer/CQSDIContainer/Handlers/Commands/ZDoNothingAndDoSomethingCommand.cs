namespace IQ.CQS.Lab.Handlers.Commands
{
	public class ZDoNothingAndDoSomethingCommand : ICommand
	{
		public ZDoNothingAndDoSomethingCommand(int numberOfIterations)
		{
			NumberOfIterations = numberOfIterations;
		}

		public int NumberOfIterations { get; }
	}
}
