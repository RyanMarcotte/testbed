using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Queries
{
	public class GetStringQueryHandler : IQueryHandler<GetStringQuery, string>
	{
		public string Handle(GetStringQuery query)
		{
			return query.Value;
		}
	}
}
