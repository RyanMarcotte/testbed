using System;
using System.Reflection;
using System.Threading;
using CQSDIContainer.UnitTests._SampleHandlers;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;

namespace CQSDIContainer.UnitTests._TestUtilities
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
			return GetHandlerInstanceForHandlerType(handlerType).GetType();
			/*switch (handlerType)
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
			}*/
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="handlerType"></param>
		/// <returns></returns>
		public static object GetHandlerInstanceForHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query:
					return new SampleQueryHandler();

				case CQSHandlerType.AsyncQuery:
					return new SampleAsyncQueryHandler();

				case CQSHandlerType.Command:
					return new SampleCommandHandler();

				case CQSHandlerType.ResultCommand_Succeeds:
					return new SampleResultCommandHandlerThatSucceeds();

				case CQSHandlerType.ResultCommand_Fails:
					return new SampleResultCommandHandlerThatFails();

				case CQSHandlerType.AsyncCommand:
					return new SampleAsyncCommandHandler();

				case CQSHandlerType.AsyncResultCommand_Succeeds:
					return new SampleAsyncResultCommandHandlerThatSucceeds();

				case CQSHandlerType.AsyncResultCommand_Fails:
					return new SampleAsyncResultCommandHandlerThatFails();

				default:
					throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
			}
		}

		public static object[] GetArgumentsForHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query:
					return new object[] { new SampleQuery() };

				case CQSHandlerType.AsyncQuery:
					return new object[] { new SampleAsyncQuery(), new CancellationToken() };

				case CQSHandlerType.Command:
				case CQSHandlerType.ResultCommand_Succeeds:
				case CQSHandlerType.ResultCommand_Fails:
					return new object[] { new SampleCommand() };

				case CQSHandlerType.AsyncCommand:
				case CQSHandlerType.AsyncResultCommand_Succeeds:
				case CQSHandlerType.AsyncResultCommand_Fails:
					return new object[] { new SampleCommand(), new CancellationToken() };

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
	}
}
