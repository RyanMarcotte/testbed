using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	// ReSharper disable once InconsistentNaming
	public class SampleQueryHandler_ReturnsReferenceType : IQueryHandler<SampleQuery_ReturnsReferenceType, string>
	{
		public const string ReturnValue = "this is a return value";

		public string Handle(SampleQuery_ReturnsReferenceType query)
		{
			return ReturnValue;
		}
	}
}