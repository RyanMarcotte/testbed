using System.Threading;
using System.Threading.Tasks;
using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	public class SampleAsyncResultCommandHandlerThatSucceeds : IAsyncResultCommandHandler<SampleCommand, int>
	{
		private static readonly Result<Unit, int> _result = Result.Succeed<Unit, int>(Unit.Value);
		public static Task<Result<Unit, int>> ReturnValue => new Task<Result<Unit, int>>(() => _result);

		public async Task<Result<Unit, int>> HandleAsync(SampleCommand command, CancellationToken cancellationToken)
		{
			return await Task.Run(() => _result, cancellationToken);
		}
	}
}