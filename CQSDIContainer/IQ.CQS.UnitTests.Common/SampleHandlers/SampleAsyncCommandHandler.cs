using System.Threading;
using System.Threading.Tasks;
using IQ.CQS.UnitTests.Framework.SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.UnitTests.Framework.SampleHandlers
{
	public class SampleAsyncCommandHandler : IAsyncCommandHandler<SampleCommand>
	{
		public static Task ReturnValue => new Task(() => { });

		public async Task HandleAsync(SampleCommand command, CancellationToken cancellationToken = new CancellationToken())
		{
			await Task.Run(() => { }, cancellationToken);
		}
	}
}