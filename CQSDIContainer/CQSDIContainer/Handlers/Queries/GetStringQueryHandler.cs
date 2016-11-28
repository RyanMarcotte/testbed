using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetStringQueryHandler : IQueryHandler<GetStringQuery, string>
	{
		public string Handle(GetStringQuery query)
		{
			return query.Value;
		}
	}
}
