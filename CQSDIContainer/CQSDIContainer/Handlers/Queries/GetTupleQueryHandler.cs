using System;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetTupleQueryHandler : IQueryHandler<GetTupleQuery, Tuple<int, string, int>>
	{
		public Tuple<int, string, int> Handle(GetTupleQuery query)
		{
			Console.WriteLine("executing query to retrieve tuple");
			return Tuple.Create(query.ID, "blah blah", query.Version);
		}
	}
}
