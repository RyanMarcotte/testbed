using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
{
	// ReSharper disable once InconsistentNaming
	public class SampleQueryHandler_ReturnsValueType : IQueryHandler<SampleQuery_ReturnsValueType, int>
	{
		public const int ReturnValue = 15;

		public int Handle(SampleQuery_ReturnsValueType query)
		{
			return ReturnValue;
		}
	}
}