using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.Mapping;

namespace TechnicalChallenge.Mappers
{
	public class CommaDelimitedStringToEnumerableCollectionOfIntegersMapper : IMapper<string, IEnumerable<int>>
	{
		public IEnumerable<int> Map(string source)
		{
			return source.Split(',').Select(x => Convert.ToInt32((string)x));
		}
	}
}