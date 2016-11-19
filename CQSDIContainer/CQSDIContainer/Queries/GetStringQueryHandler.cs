using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
{
	public class GetStringQueryHandler : IQueryHandler<GetStringQuery, string>
	{
		public string Handle(GetStringQuery query)
		{
			return query.Value;
		}
	}
}
