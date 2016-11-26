using System;
using System.Threading.Tasks;
using IQ.CQS.Lab.Commands;
using IQ.CQS.Lab.Queries;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab
{
	public interface IRQApplicationServiceMock
	{
		Task DoStuff();
	}

	internal class MockRQApplicationServiceUsingDI : IRQApplicationServiceMock
	{
		private readonly IQueryHandler<GetIntegerQuery, int> _syncQueryHandlerForValueType;
		private readonly IQueryHandler<GetStringQuery, string> _syncQueryHandlerForReferenceType;
		private readonly IAsyncQueryHandler<GetIntegerAsyncQuery, int> _asyncQueryHandlerForValueType;
		private readonly IAsyncQueryHandler<GetStringAsyncQuery, string> _asyncQueryHandlerForReferenceType;
		private readonly ICommandHandler<DoNothingAndDoSomethingCommand> _syncCommandHandler;
		private readonly IAsyncCommandHandler<DoSomethingAsyncCommand> _asyncCommandHandler;
		private readonly IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode> _syncCommandHandlerWithResult;
		private readonly IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode> _asyncCommandHandlerWithResult;

		public MockRQApplicationServiceUsingDI(
			IQueryHandler<GetIntegerQuery, int> syncQueryHandlerForValueType,
			IQueryHandler<GetStringQuery, string> syncQueryHandlerForReferenceType,
			IAsyncQueryHandler<GetIntegerAsyncQuery, int> asyncQueryHandlerForValueType,
			IAsyncQueryHandler<GetStringAsyncQuery, string> asyncQueryHandlerForReferenceType,
			ICommandHandler<DoNothingAndDoSomethingCommand> syncCommandHandler,
			IAsyncCommandHandler<DoSomethingAsyncCommand> asyncCommandHandler,
			IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode> syncCommandHandlerWithResult,
			IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode> asyncCommandHandlerWithResult)
		{
			_syncQueryHandlerForValueType = syncQueryHandlerForValueType;
			_syncQueryHandlerForReferenceType = syncQueryHandlerForReferenceType;
			_asyncQueryHandlerForValueType = asyncQueryHandlerForValueType;
			_asyncQueryHandlerForReferenceType = asyncQueryHandlerForReferenceType;
			_syncCommandHandler = syncCommandHandler;
			_asyncCommandHandler = asyncCommandHandler;
			_syncCommandHandlerWithResult = syncCommandHandlerWithResult;
			_asyncCommandHandlerWithResult = asyncCommandHandlerWithResult;
		}

		public async Task DoStuff()
		{
			try
			{
				Console.WriteLine("--[[ query handler test ]]--");
				var nameOfSyncQueryHandlerForValueType = _syncQueryHandlerForValueType.GetType().FullName;
				const int intSyncQueryParameter = 11;
				Console.WriteLine($"Result of {nameOfSyncQueryHandlerForValueType} = {_syncQueryHandlerForValueType.Handle(new GetIntegerQuery(intSyncQueryParameter))}");
				Console.WriteLine($"Result of {nameOfSyncQueryHandlerForValueType} = {_syncQueryHandlerForValueType.Handle(new GetIntegerQuery(intSyncQueryParameter))}");
				Console.WriteLine($"Result of {nameOfSyncQueryHandlerForValueType} = {_syncQueryHandlerForValueType.Handle(new GetIntegerQuery(intSyncQueryParameter))}");
				Console.WriteLine($"Result of {nameOfSyncQueryHandlerForValueType} = {_syncQueryHandlerForValueType.Handle(new GetIntegerQuery(intSyncQueryParameter))}");
				Console.WriteLine();

				/*var nameOfAsyncQueryHandlerTypeForReferenceType = _syncQueryHandlerForReferenceType.GetType().FullName;
				const string stringQueryParameter = "this is a string";
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerTypeForReferenceType} = {_syncQueryHandlerForReferenceType.Handle(new GetStringQuery(stringQueryParameter))}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerTypeForReferenceType} = {_syncQueryHandlerForReferenceType.Handle(new GetStringQuery(stringQueryParameter))}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerTypeForReferenceType} = {_syncQueryHandlerForReferenceType.Handle(new GetStringQuery(stringQueryParameter))}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerTypeForReferenceType} = {_syncQueryHandlerForReferenceType.Handle(new GetStringQuery(stringQueryParameter))}");

				var nameOfAsyncQueryHandlerForValueType = _asyncQueryHandlerForValueType.GetType().FullName;
				const int intAsyncQueryParameter = 15;
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerForValueType} = {await _asyncQueryHandlerForValueType.HandleAsync(new GetIntegerAsyncQuery(intAsyncQueryParameter))}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerForValueType} = {await _asyncQueryHandlerForValueType.HandleAsync(new GetIntegerAsyncQuery(intAsyncQueryParameter))}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerForValueType} = {await _asyncQueryHandlerForValueType.HandleAsync(new GetIntegerAsyncQuery(intAsyncQueryParameter))}");

				var nameOfAsyncQueryHandlerToReferenceType = _asyncQueryHandlerForReferenceType.GetType().FullName;
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerToReferenceType} = {await _asyncQueryHandlerForReferenceType.HandleAsync(new GetStringAsyncQuery())}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerToReferenceType} = {await _asyncQueryHandlerForReferenceType.HandleAsync(new GetStringAsyncQuery())}");
				Console.WriteLine($"Result of {nameOfAsyncQueryHandlerToReferenceType} = {await _asyncQueryHandlerForReferenceType.HandleAsync(new GetStringAsyncQuery())}");*/

				for (int n = 4; n < 8; ++n)
				{
					_syncCommandHandler.Handle(new DoNothingAndDoSomethingCommand(n / 2));
					Console.WriteLine();
				}

				/*Console.WriteLine("--[[ async command handler test ]]--");
				await _asyncCommandHandler.HandleAsync(new DoSomethingAsyncCommand());
				Console.WriteLine();

				Console.WriteLine("--[[ synchronous command (with result) handler test ]]--");
				Console.WriteLine($"Result of {_syncCommandHandlerWithResult.GetType().FullName} = {_syncCommandHandlerWithResult.Handle(new DoSomethingWithResultCommand(3, 0))}");
				Console.WriteLine();

				Console.WriteLine("--[[ async command (with result) handler test ]]--");
				Console.WriteLine($"Result of {_asyncCommandHandlerWithResult.GetType().FullName} = {await _asyncCommandHandlerWithResult.HandleAsync(new DoSomethingAsyncWithResultCommand(), new CancellationToken())}");
				Console.WriteLine();*/
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}
	}
}