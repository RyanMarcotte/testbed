using System;
using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.Commands;
using CQSDIContainer.Queries;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer
{
	internal class MockRQApplicationServiceUsingDI
	{
		private readonly IQueryHandler<GetIntegerQuery, int> _syncQueryHandler;
		private readonly IAsyncQueryHandler<GetStringAsyncQuery, string> _asyncQueryHandler;
		private readonly ICommandHandler<DoNothingAndDoSomethingCommand> _syncCommandHandler;
		private readonly IAsyncCommandHandler<DoSomethingAsyncCommand> _asyncCommandHandler;
		private readonly IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode> _syncCommandHandlerWithResult;
		private readonly IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode> _asyncCommandHandlerWithResult;

		public MockRQApplicationServiceUsingDI(
			IQueryHandler<GetIntegerQuery, int> syncQueryHandler,
			IAsyncQueryHandler<GetStringAsyncQuery, string> asyncQueryHandler,
			ICommandHandler<DoNothingAndDoSomethingCommand> syncCommandHandler,
			IAsyncCommandHandler<DoSomethingAsyncCommand> asyncCommandHandler,
			IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode> syncCommandHandlerWithResult,
			IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode> asyncCommandHandlerWithResult)
		{
			_syncQueryHandler = syncQueryHandler;
			_asyncQueryHandler = asyncQueryHandler;
			_syncCommandHandler = syncCommandHandler;
			_asyncCommandHandler = asyncCommandHandler;
			_syncCommandHandlerWithResult = syncCommandHandlerWithResult;
			_asyncCommandHandlerWithResult = asyncCommandHandlerWithResult;
		}

		public async Task DoStuff()
		{
			Console.WriteLine($"Result of {_syncQueryHandler.GetType().FullName} = {_syncQueryHandler.Handle(new GetIntegerQuery(11))}");
			Console.WriteLine($"Result of {_syncQueryHandler.GetType().FullName} = {_syncQueryHandler.Handle(new GetIntegerQuery(11))}");
			Console.WriteLine($"Result of {_syncQueryHandler.GetType().FullName} = {_syncQueryHandler.Handle(new GetIntegerQuery(11))}");
			Console.WriteLine($"Result of {_syncQueryHandler.GetType().FullName} = {_syncQueryHandler.Handle(new GetIntegerQuery(11))}");
			Console.WriteLine($"Result of {_asyncQueryHandler.GetType().FullName} = {await _asyncQueryHandler.HandleAsync(new GetStringAsyncQuery())}");

			_syncCommandHandler.Handle(new DoNothingAndDoSomethingCommand(3));
			await _asyncCommandHandler.HandleAsync(new DoSomethingAsyncCommand());
			Console.WriteLine($"Result of {_syncCommandHandlerWithResult.GetType().FullName} = {_syncCommandHandlerWithResult.Handle(new DoSomethingWithResultCommand(3, 0))}");
			Console.WriteLine($"Result of {_asyncCommandHandlerWithResult.GetType().FullName} = {await _asyncCommandHandlerWithResult.HandleAsync(new DoSomethingAsyncWithResultCommand(), new CancellationToken())}");

			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}
	}
}