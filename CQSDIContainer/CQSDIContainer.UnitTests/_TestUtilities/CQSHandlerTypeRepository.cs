using System;
using System.Collections.Generic;
using System.Linq;

namespace CQSDIContainer.UnitTests._TestUtilities
{
	/// <summary>
	/// Utility class for retrieving common collections of <see cref="CQSHandlerType"/> enum values.
	/// </summary>
	public static class CQSHandlerTypeRepository
	{
		public static IEnumerable<CQSHandlerType> GetHandlerTypes(CQSHandlerTypeSelector selector)
		{
			if (selector.HasFlag(CQSHandlerTypeSelector.Query_ReturnsValueType))
				yield return CQSHandlerType.Query_ReturnsValueType;
			if (selector.HasFlag(CQSHandlerTypeSelector.Query_ReturnsReferenceType))
				yield return CQSHandlerType.Query_ReturnsReferenceType;
			if (selector.HasFlag(CQSHandlerTypeSelector.AsyncQuery_ReturnsValueType))
				yield return CQSHandlerType.AsyncQuery_ReturnsValueType;
			if (selector.HasFlag(CQSHandlerTypeSelector.AsyncQuery_ReturnsReferenceType))
				yield return CQSHandlerType.AsyncQuery_ReturnsReferenceType;
			if (selector.HasFlag(CQSHandlerTypeSelector.Command))
				yield return CQSHandlerType.Command;
			if (selector.HasFlag(CQSHandlerTypeSelector.AsyncCommand))
				yield return CQSHandlerType.AsyncCommand;
			if (selector.HasFlag(CQSHandlerTypeSelector.ResultCommand_Succeeds))
				yield return CQSHandlerType.ResultCommand_Succeeds;
			if (selector.HasFlag(CQSHandlerTypeSelector.ResultCommand_Fails))
				yield return CQSHandlerType.ResultCommand_Fails;
			if (selector.HasFlag(CQSHandlerTypeSelector.AsyncResultCommand_Succeeds))
				yield return CQSHandlerType.AsyncResultCommand_Succeeds;
			if (selector.HasFlag(CQSHandlerTypeSelector.AsyncResultCommand_Fails))
				yield return CQSHandlerType.AsyncResultCommand_Fails;
		}
	}
}