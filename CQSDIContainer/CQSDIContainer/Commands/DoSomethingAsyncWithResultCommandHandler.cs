﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Commands
{
	public class DoSomethingAsyncWithResultCommandHandler : IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode>
	{
		public async Task<Result<Unit, DoSomethingAsyncWithResultCommandHandlerErrorCode>> HandleAsync(DoSomethingAsyncWithResultCommand command, CancellationToken cancellationToken)
		{
			return await Task.Run(() =>
			{
				Console.WriteLine();
				Console.WriteLine("HANDLING ASYNC COMMAND WITH RESULT");
				Console.WriteLine();

				return Result.Fail<Unit, DoSomethingAsyncWithResultCommandHandlerErrorCode>(DoSomethingAsyncWithResultCommandHandlerErrorCode.ErrorOccurred);
			}, cancellationToken);
		}
	}

	public enum DoSomethingAsyncWithResultCommandHandlerErrorCode
	{
		ErrorOccurred
	}
}
