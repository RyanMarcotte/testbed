using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
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