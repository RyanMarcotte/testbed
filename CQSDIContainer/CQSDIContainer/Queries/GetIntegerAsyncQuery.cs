using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
{
	public class GetIntegerAsyncQuery : IQuery<int>
	{
		public GetIntegerAsyncQuery(int value)
		{
			Value = value;
		}

		public int Value { get; }
	}
}
