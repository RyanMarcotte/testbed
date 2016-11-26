using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
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