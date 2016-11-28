using System;
using System.Threading;
using System.Threading.Tasks;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Commands
{
	public class DoSomethingAsyncCommandHandler : IAsyncCommandHandler<DoSomethingAsyncCommand>
	{
		public async Task HandleAsync(DoSomethingAsyncCommand command, CancellationToken cancellationToken = new CancellationToken())
		{
			await Task.Run(() =>
			{
				Console.WriteLine();
				Console.WriteLine("HANDLING ASYNC COMMAND!!");
				Console.WriteLine();
			}, cancellationToken);
		}
	}
}
