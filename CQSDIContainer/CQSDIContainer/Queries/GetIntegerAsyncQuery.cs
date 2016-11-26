using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Queries
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
