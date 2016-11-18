using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Commands
{
	public class DoSomethingAsyncCommandHandler : IAsyncCommandHandler<DoSomethingAsyncCommand>
	{
		public async Task HandleAsync(DoSomethingAsyncCommand command, CancellationToken cancellationToken = new CancellationToken())
		{
			await Task.Run(() => Console.WriteLine("this is an async command"), cancellationToken);
		}
	}
}
