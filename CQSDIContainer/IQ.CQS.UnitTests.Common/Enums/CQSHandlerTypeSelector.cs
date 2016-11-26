using System;

// ReSharper disable InconsistentNaming

namespace IQ.CQS.UnitTests.Framework.Enums
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum CQSHandlerTypeSelector
	{
		None = 0,
		
		Query_ReturnsValueType = 1,
		
		Query_ReturnsReferenceType = 2,
		
		/// <summary>
		/// Only include synchronous query handlers.
		/// </summary>
		SynchronousQueryHandlersOnly = Query_ReturnsValueType | Query_ReturnsReferenceType,
		
		AsyncQuery_ReturnsValueType = 4,
		
		AsyncQuery_ReturnsReferenceType = 8,
		
		/// <summary>
		/// Only include asynchronous query handlers.
		/// </summary>
		AsyncQueryHandlersOnly = AsyncQuery_ReturnsValueType | AsyncQuery_ReturnsReferenceType,
		
		/// <summary>
		/// All synchronous and asynchronous query handlers.
		/// </summary>
		AllQueryHandlers = SynchronousQueryHandlersOnly | AsyncQueryHandlersOnly,
		
		Command = 16,

		AsyncCommand = 32,

		ResultCommand_Succeeds = 64,

		ResultCommand_Fails = 128,

		AsyncResultCommand_Succeeds = 256,

		AsyncResultCommand_Fails = 512,

		/// <summary>
		/// All synchronous and asynchronous command handlers.
		/// </summary>
		AllCommandHandlers = Command | AsyncCommand | ResultCommand_Succeeds | ResultCommand_Fails | AsyncResultCommand_Succeeds | AsyncResultCommand_Fails,

		/// <summary>
		/// All handlers.
		/// </summary>
		AllHandlers = AllQueryHandlers | AllCommandHandlers
	}
}