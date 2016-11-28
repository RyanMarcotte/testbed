using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetStringQuery : IQuery<string>
	{
		public GetStringQuery(string value)
		{
			Value = value;
		}

		public string Value { get; }
	}
}
