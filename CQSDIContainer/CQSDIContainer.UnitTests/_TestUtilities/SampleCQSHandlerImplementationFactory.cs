using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests._SampleHandlers;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common;

namespace CQSDIContainer.UnitTests._TestUtilities
{
	/// <summary>
	/// Utility class for generating data related to sample CQS handler implementations provided by the unit test suite (see '_SampleHandlers' folder).
	/// </summary>
	public static class SampleCQSHandlerImplementationFactory
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
				case CQSHandlerType.Query_ReturnsValueType:
					return typeof(SampleQueryHandler_ReturnsValueType).GetMethod(nameof(SampleQueryHandler_ReturnsValueType.Handle));

				case CQSHandlerType.Query_ReturnsReferenceType:
					return typeof(SampleQueryHandler_ReturnsReferenceType).GetMethod(nameof(SampleQueryHandler_ReturnsReferenceType.Handle));

				case CQSHandlerType.AsyncQuery_ReturnsValueType:
					return typeof(SampleAsyncQueryHandler_ReturnsValueType).GetMethod(nameof(SampleAsyncQueryHandler_ReturnsValueType.HandleAsync));

				case CQSHandlerType.AsyncQuery_ReturnsReferenceType:
					return typeof(SampleAsyncQueryHandler_ReturnsReferenceType).GetMethod(nameof(SampleAsyncQueryHandler_ReturnsReferenceType.HandleAsync));

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
		public static Type GetSampleImplementationClassTypeForHandlerType(CQSHandlerType handlerType)
		{
			return GetNewHandlerInstanceForHandlerType(handlerType).GetType();
		}

		/// <summary>
		/// Retrieves an instance of a CQS handler type's sample implementation.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
		public static object GetNewHandlerInstanceForHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query_ReturnsValueType:
					return new SampleQueryHandler_ReturnsValueType();

				case CQSHandlerType.Query_ReturnsReferenceType:
					return new SampleQueryHandler_ReturnsReferenceType();

				case CQSHandlerType.AsyncQuery_ReturnsValueType:
					return new SampleAsyncQueryHandler_ReturnsValueType();

				case CQSHandlerType.AsyncQuery_ReturnsReferenceType:
					return new SampleAsyncQueryHandler_ReturnsReferenceType();

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

		/// <summary>
		/// Retrieves the argument list used when calling Handle / HandleAsync for a CQS handler type's sample implementation.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
		public static object[] GetArgumentsUsedForHandleAndHandleAsyncMethodsForHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query_ReturnsValueType:
					return new object[] { new SampleQuery_ReturnsValueType() };

				case CQSHandlerType.Query_ReturnsReferenceType:
					return new object[] { new SampleQuery_ReturnsReferenceType() };

				case CQSHandlerType.AsyncQuery_ReturnsValueType:
					return new object[] { new SampleAsyncQuery_ReturnsValueType(), new CancellationToken() };

				case CQSHandlerType.AsyncQuery_ReturnsReferenceType:
					return new object[] { new SampleAsyncQuery_ReturnsReferenceType(), new CancellationToken() };

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
		/// Retrieves the return value of a CQS handler type's sample implementation.
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
		public static object GetReturnValueForHandlerType(CQSHandlerType handlerType)
		{
			switch (handlerType)
			{
				case CQSHandlerType.Query_ReturnsValueType:
					return SampleQueryHandler_ReturnsValueType.ReturnValue;

				case CQSHandlerType.Query_ReturnsReferenceType:
					return SampleQueryHandler_ReturnsReferenceType.ReturnValue;

				case CQSHandlerType.AsyncQuery_ReturnsValueType:
					var asyncQueryWithValueTypeResultReturnValue = SampleAsyncQueryHandler_ReturnsValueType.ReturnValue;
					asyncQueryWithValueTypeResultReturnValue.RunSynchronously();
					return asyncQueryWithValueTypeResultReturnValue;

				case CQSHandlerType.AsyncQuery_ReturnsReferenceType:
					var asyncQueryWithReferenceTypeResultReturnValue = SampleAsyncQueryHandler_ReturnsReferenceType.ReturnValue;
					asyncQueryWithReferenceTypeResultReturnValue.RunSynchronously();
					return asyncQueryWithReferenceTypeResultReturnValue;

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

		/// <summary>
		/// Returns the underlying return value type of a CQS handler type's sample implementation.  If the handler returns Task&lt;T>, this method returns T."/>
		/// </summary>
		/// <param name="handlerType">The handler type.</param>
		/// <returns></returns>
		public static Type GetUnderlyingReturnValueTypeForHandlerType(CQSHandlerType handlerType)
		{
			var methodInfo = GetMethodInfoFromHandlerType(handlerType);
			var returnValueType = methodInfo.ReturnType;
			if (!returnValueType.IsGenericType || returnValueType.GetGenericTypeDefinition() != typeof(Task<>))
				return returnValueType == typeof(Task) ? typeof(void) : returnValueType;

			return returnValueType.GetGenericArguments()[0];
		}
	}
}
