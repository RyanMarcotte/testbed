using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Attributes;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
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
