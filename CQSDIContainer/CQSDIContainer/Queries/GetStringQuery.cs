using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Queries
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
