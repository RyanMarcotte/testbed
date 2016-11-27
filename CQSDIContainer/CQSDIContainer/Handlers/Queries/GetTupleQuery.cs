using System;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
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
