using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
{
	public class GetTupleQuery : IQuery<Tuple<int, string, int>>
	{
		public GetTupleQuery(int id, int version)
		{
			ID = id;
			Version = version;
		}

		public int ID { get; }
		public int Version { get; }
	}
}
