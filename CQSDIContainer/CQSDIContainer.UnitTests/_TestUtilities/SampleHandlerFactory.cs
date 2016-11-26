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
		/// <summary>
		/// Retrieves method metadata for a specific CQS handler type's Handle / HandleAsync method.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
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

				case CQSHandlerType.ResultCommand_Succeeds:
					return typeof(SampleResultCommandHandlerThatSucceeds).GetMethod(nameof(SampleResultCommandHandlerThatSucceeds.Handle));

				case CQSHandlerType.ResultCommand_Fails:
					return typeof(SampleResultCommandHandlerThatFails).GetMethod(nameof(SampleResultCommandHandlerThatFails.Handle));

				case CQSHandlerType.AsyncCommand:
					return typeof(SampleAsyncCommandHandler).GetMethod(nameof(SampleAsyncCommandHandler.HandleAsync));

				case CQSHandlerType.AsyncResultCommand_Succeeds:
					return typeof(SampleAsyncResultCommandHandlerThatSucceeds).GetMethod(nameof(SampleAsyncResultCommandHandlerThatSucceeds.HandleAsync));

				case CQSHandlerType.AsyncResultCommand_Fails:
					return typeof(SampleAsyncResultCommandHandlerThatFails).GetMethod(nameof(SampleAsyncResultCommandHandlerThatFails.HandleAsync));

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		/// <summary>
		/// Retrieves the type of a CQS handler type's sample implementation.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
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

				case CQSHandlerType.ResultCommand_Succeeds:
					return typeof(SampleResultCommandHandlerThatSucceeds);

				case CQSHandlerType.ResultCommand_Fails:
					return typeof(SampleResultCommandHandlerThatFails);

				case CQSHandlerType.AsyncCommand:
					return typeof(SampleAsyncCommandHandler);

				case CQSHandlerType.AsyncResultCommand_Succeeds:
					return typeof(SampleAsyncResultCommandHandlerThatSucceeds);

				case CQSHandlerType.AsyncResultCommand_Fails:
					return typeof(SampleAsyncResultCommandHandlerThatFails);

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		/// <summary>
		/// Retrieves the sample return value of a CQS handler type.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
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

				case CQSHandlerType.ResultCommand_Succeeds:
					return SampleResultCommandHandlerThatSucceeds.ReturnValue;

				case CQSHandlerType.ResultCommand_Fails:
					return SampleResultCommandHandlerThatFails.ReturnValue;

				case CQSHandlerType.AsyncCommand:
					var asyncCommandReturnValue = SampleAsyncCommandHandler.ReturnValue;
					asyncCommandReturnValue.RunSynchronously();
					return asyncCommandReturnValue;

				case CQSHandlerType.AsyncResultCommand_Succeeds:
					var asyncResultCommandSuccessReturnValue = SampleAsyncResultCommandHandlerThatSucceeds.ReturnValue;
					asyncResultCommandSuccessReturnValue.RunSynchronously();
					return asyncResultCommandSuccessReturnValue;

				case CQSHandlerType.AsyncResultCommand_Fails:
					var asyncResultCommandFailReturnValue = SampleAsyncResultCommandHandlerThatFails.ReturnValue;
					asyncResultCommandFailReturnValue.RunSynchronously();
					return asyncResultCommandFailReturnValue;

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		#region Sample Implementations

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

		private class SampleResultCommandHandlerThatSucceeds : IResultCommandHandler<int, int>
		{
			public static Result<Unit, int> ReturnValue => Result.Succeed<Unit, int>(Unit.Value);

			public Result<Unit, int> Handle(int command)
			{
				return ReturnValue;
			}
		}

		private class SampleResultCommandHandlerThatFails : IResultCommandHandler<int, int>
		{
			public static Result<Unit, int> ReturnValue => Result.Fail<Unit, int>(17);

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

		private class SampleAsyncResultCommandHandlerThatSucceeds : IAsyncResultCommandHandler<int, int>
		{
			private static readonly Result<Unit, int> _result = Result.Succeed<Unit, int>(Unit.Value);
			public static Task<Result<Unit, int>> ReturnValue => new Task<Result<Unit, int>>(() => _result);

			public async Task<Result<Unit, int>> HandleAsync(int command, CancellationToken cancellationToken)
			{
				return await Task.Run(() => _result, cancellationToken);
			}
		}

		private class SampleAsyncResultCommandHandlerThatFails : IAsyncResultCommandHandler<int, int>
		{
			private static readonly Result<Unit, int> _result = Result.Fail<Unit, int>(42);
			public static Task<Result<Unit, int>> ReturnValue => new Task<Result<Unit, int>>(() => _result);

			public async Task<Result<Unit, int>> HandleAsync(int command, CancellationToken cancellationToken)
			{
				return await Task.Run(() => _result, cancellationToken);
			}
		}

		#endregion
	}
}
