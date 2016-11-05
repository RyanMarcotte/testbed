using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
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
