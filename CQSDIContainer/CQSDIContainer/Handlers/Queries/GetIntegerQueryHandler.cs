using System;
using IQ.CQS.Attributes;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	[LogExecutionTime]
	public class GetIntegerQueryHandler : IQueryHandler<GetIntegerQuery, int>
	{
		private static readonly Random _random = new Random();

		public int Handle(GetIntegerQuery query)
		{
			Console.WriteLine("executing query to retrieve int");
			return _random.Next(1, 10);
		}
	}
}
