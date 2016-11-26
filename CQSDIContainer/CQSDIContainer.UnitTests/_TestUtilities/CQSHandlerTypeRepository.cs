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
		/// <summary>
		/// Gets the collection of <see cref="CQSHandlerType"/> enum values corresponding to query handler types.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<CQSHandlerType> AllQueryTypes()
		{
			yield return CQSHandlerType.Query;
			yield return CQSHandlerType.AsyncQuery;
		}

		/// <summary>
		/// Gets the collection of <see cref="CQSHandlerType"/> enum values corresponding to command handler types.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<CQSHandlerType> AllCommandTypes()
		{
			yield return CQSHandlerType.Command;
			yield return CQSHandlerType.AsyncCommand;
			yield return CQSHandlerType.ResultCommand_Succeeds;
			yield return CQSHandlerType.ResultCommand_Fails;
			yield return CQSHandlerType.AsyncResultCommand_Succeeds;
			yield return CQSHandlerType.AsyncResultCommand_Fails;
		}

		/// <summary>
		/// Gets the collection of all <see cref="CQSHandlerType"/> values.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<CQSHandlerType> AllTypes()
		{
			return Enum.GetValues(typeof(CQSHandlerType)).Cast<CQSHandlerType>();
		}
	}
}