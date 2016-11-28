namespace IQ.CQS.Lab.Handlers.Commands
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
