using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
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
