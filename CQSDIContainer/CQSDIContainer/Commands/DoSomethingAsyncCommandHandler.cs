﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Commands
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
