﻿using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Queries
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
