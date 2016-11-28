using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetIntegerAsyncQuery : IQuery<int>
	{
		public GetIntegerAsyncQuery(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}
}
