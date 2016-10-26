using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Attributes;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
{
	[LogExecutionTimeToConsole]
	public class GetIntegerQueryHandler : IQueryHandler<GetIntegerQuery, int>
	{
		private static readonly Random _random = new Random();

		public int Handle(GetIntegerQuery query)
		{
			return _random.Next(1, 10);
		}
	}
}
