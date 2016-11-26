using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
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