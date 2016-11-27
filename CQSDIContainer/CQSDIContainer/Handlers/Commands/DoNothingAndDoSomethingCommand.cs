namespace IQ.CQS.Lab.Handlers.Commands
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
