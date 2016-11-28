using System.Threading;
using System.Threading.Tasks;
using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	// ReSharper disable once InconsistentNaming
	public class SampleAsyncQueryHandler_ReturnsValueType : IAsyncQueryHandler<SampleAsyncQuery_ReturnsValueType, int>
	{
		private static readonly int _result = 156;
		public static Task<int> ReturnValue => new Task<int>(() => _result);

		public async Task<int> HandleAsync(SampleAsyncQuery_ReturnsValueType query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => _result, cancellationToken);
		}
	}
}