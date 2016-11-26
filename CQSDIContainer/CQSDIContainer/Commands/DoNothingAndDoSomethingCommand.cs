namespace IQ.CQS.Lab.Commands
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
