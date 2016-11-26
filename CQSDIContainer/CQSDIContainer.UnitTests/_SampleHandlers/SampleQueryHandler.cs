using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
{
	public class SampleQueryHandler : IQueryHandler<SampleQuery, int>
	{
		public const int ReturnValue = 15;

		public int Handle(SampleQuery query)
		{
			return ReturnValue;
		}
	}
}