using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
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