using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
{
	public class SampleAsyncResultCommandHandlerThatFails : IAsyncResultCommandHandler<SampleCommand, int>
	{
		private static readonly Result<Unit, int> _result = Result.Fail<Unit, int>(42);
		public static Task<Result<Unit, int>> ReturnValue => new Task<Result<Unit, int>>(() => _result);

		public async Task<Result<Unit, int>> HandleAsync(SampleCommand command, CancellationToken cancellationToken)
		{
			return await Task.Run(() => _result, cancellationToken);
		}
	}
}