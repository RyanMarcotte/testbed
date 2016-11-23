using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.TestUtilities;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.TestUtilities
{
	public static class SampleHandlerFactory
	{
		public static MethodInfo GetMethodInfoFromHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query:
					return typeof(SampleQueryHandler).GetMethod(nameof(SampleQueryHandler.Handle));

				case CQSHandlerType.AsyncQuery:
					return typeof(SampleAsyncQueryHandler).GetMethod(nameof(SampleAsyncQueryHandler.HandleAsync));

				case CQSHandlerType.Command:
					return typeof(SampleCommandHandler).GetMethod(nameof(SampleCommandHandler.Handle));

				case CQSHandlerType.ResultCommand:
					return typeof(SampleResultCommandHandler).GetMethod(nameof(SampleResultCommandHandler.Handle));

				case CQSHandlerType.AsyncCommand:
					return typeof(SampleAsyncCommandHandler).GetMethod(nameof(SampleAsyncCommandHandler.HandleAsync));

				case CQSHandlerType.AsyncResultCommand:
					return typeof(SampleAsyncResultCommandHandler).GetMethod(nameof(SampleAsyncResultCommandHandler.HandleAsync));

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		public static Type GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query:
					return typeof(SampleQueryHandler);
				case CQSHandlerType.AsyncQuery:
					return typeof(SampleAsyncQueryHandler);
				case CQSHandlerType.Command:
					return typeof(SampleCommandHandler);
				case CQSHandlerType.ResultCommand:
					return typeof(SampleResultCommandHandler);
				case CQSHandlerType.AsyncCommand:
					return typeof(SampleAsyncCommandHandler);
				case CQSHandlerType.AsyncResultCommand:
					return typeof(SampleAsyncResultCommandHandler);
				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		public static object GetReturnValueForHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query:
					return SampleQueryHandler.ReturnValue;
				
				case CQSHandlerType.AsyncQuery:
					var asyncQueryReturnValue = SampleAsyncQueryHandler.ReturnValue;
					asyncQueryReturnValue.RunSynchronously();
					return asyncQueryReturnValue;

				case CQSHandlerType.Command:
					return SampleCommandHandler.ReturnValue;

				case CQSHandlerType.ResultCommand:
					return SampleResultCommandHandler.ReturnValue;

				case CQSHandlerType.AsyncCommand:
					var asyncCommandReturnValue = SampleAsyncCommandHandler.ReturnValue;
					asyncCommandReturnValue.RunSynchronously();
					return asyncCommandReturnValue;

				case CQSHandlerType.AsyncResultCommand:
					var asyncResultCommandReturnValue = SampleAsyncResultCommandHandler.ReturnValue;
					asyncResultCommandReturnValue.RunSynchronously();
					return asyncResultCommandReturnValue;

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		private static Task RunAsyncTask(Task task)
		{
			task.RunSynchronously();
			return task;
		}

		// ReSharper disable once ClassNeverInstantiated.Local
		private class SampleQuery : IQuery<int>
		{
		}

		private class SampleQueryHandler : IQueryHandler<SampleQuery, int>
		{
			public const int ReturnValue = 15;

			public int Handle(SampleQuery query)
			{
				return ReturnValue;
			}
		}

		private class SampleAsyncQueryHandler : IAsyncQueryHandler<SampleQuery, int>
		{
			private static readonly int _result = 156;
			public static Task<int> ReturnValue => new Task<int>(() => _result);

			public async Task<int> HandleAsync(SampleQuery query, CancellationToken cancellationToken = new CancellationToken())
			{
				return await Task.Run(() => _result, cancellationToken);
			}
		}

		private class SampleCommandHandler : ICommandHandler<int>
		{
			public static readonly object ReturnValue = typeof(void);
			
			public void Handle(int command)
			{

			}
		}

		private class SampleResultCommandHandler : IResultCommandHandler<int, int>
		{
			public static Result<Unit, int> ReturnValue => Result.Succeed<Unit, int>(Unit.Value);

			public Result<Unit, int> Handle(int command)
			{
				return ReturnValue;
			}
		}

		private class SampleAsyncCommandHandler : IAsyncCommandHandler<int>
		{
			public static Task ReturnValue => new Task(() => { });

			public async Task HandleAsync(int command, CancellationToken cancellationToken = new CancellationToken())
			{
				await Task.Run(() => { }, cancellationToken);
			}
		}

		private class SampleAsyncResultCommandHandler : IAsyncResultCommandHandler<int, int>
		{
			private static readonly Result<Unit, int> _result = Result.Succeed<Unit, int>(Unit.Value);
			public static Task<Result<Unit, int>> ReturnValue => new Task<Result<Unit, int>>(() => _result);

			public async Task<Result<Unit, int>> HandleAsync(int command, CancellationToken cancellationToken)
			{
				return await Task.Run(() => _result, cancellationToken);
			}
		}
	}
}
