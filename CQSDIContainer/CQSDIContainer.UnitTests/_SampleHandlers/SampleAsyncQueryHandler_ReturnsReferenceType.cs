using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
{
	// ReSharper disable once InconsistentNaming
	public class SampleAsyncQueryHandler_ReturnsReferenceType : IAsyncQueryHandler<SampleAsyncQuery_ReturnsReferenceType, string>
	{
		private static readonly string _result = "the result";
		public static Task<string> ReturnValue => new Task<string>(() => _result);

		public async Task<string> HandleAsync(SampleAsyncQuery_ReturnsReferenceType query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => _result, cancellationToken);
		}
	}
}