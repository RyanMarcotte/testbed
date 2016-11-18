using CQSDIContainer.Commands;
using CQSDIContainer.Queries;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer
{
	internal class MockRQApplicationServiceUsingDI
	{
		private readonly IQueryHandler<GetIntegerQuery, int> _syncQueryHandler;
		private readonly IAsyncQueryHandler<GetStringAsyncQuery, string> _asyncQueryHandler;
		private readonly ICommandHandler<DoNothingAndDoSomethingCommand> _syncCommanderHandler;
		private readonly IAsyncCommandHandler<DoSomethingAsyncCommand> _asyncCommandHandler;
		private readonly IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode> _syncCommandHandlerWithResult;
		private readonly IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode> _asyncCommandHandlerWithResult;

		public MockRQApplicationServiceUsingDI(
			IQueryHandler<GetIntegerQuery, int> syncQueryHandler,
			IAsyncQueryHandler<GetStringAsyncQuery, string> asyncQueryHandler,
			ICommandHandler<DoNothingAndDoSomethingCommand> syncCommanderHandler,
			IAsyncCommandHandler<DoSomethingAsyncCommand> asyncCommandHandler,
			IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode> syncCommandHandlerWithResult,
			IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode> asyncCommandHandlerWithResult)
		{
			_syncQueryHandler = syncQueryHandler;
			_asyncQueryHandler = asyncQueryHandler;
			_syncCommanderHandler = syncCommanderHandler;
			_asyncCommandHandler = asyncCommandHandler;
			_syncCommandHandlerWithResult = syncCommandHandlerWithResult;
			_asyncCommandHandlerWithResult = asyncCommandHandlerWithResult;
		}

		public void DoStuff()
		{
			
		}
	}
}