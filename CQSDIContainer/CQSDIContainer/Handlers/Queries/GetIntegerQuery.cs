﻿using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetIntegerQuery : IQuery<int>
	{
		public GetIntegerQuery(int id)
		{
			ID = id;
		}

		public int ID { get; }
	}
}
