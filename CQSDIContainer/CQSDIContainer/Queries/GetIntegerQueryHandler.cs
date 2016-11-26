﻿using System;
using IQ.CQS.Attributes;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Queries
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
