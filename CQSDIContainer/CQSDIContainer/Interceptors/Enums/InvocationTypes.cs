using System;
using System.Collections.Generic;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Interceptors.Enums
{
	/// <summary>
	/// Enumeration of CQS handler invocation types.
	/// </summary>
	[Flags]
	public enum InvocationTypes
	{
		None = 0,
		Query = 1,
		AsyncQuery = 2,
		Command = 4,
		AsyncCommand = 8,
		ResultCommand = 16,
		AsyncResultCommand = 32
	}

	/// <summary>
	/// Extension methods for the <see cref="InvocationTypes"/> enum.
	/// </summary>
	public static class InvocationTypesExtensions
	{
		/// <summary>
		/// Checks if more than one invocation type flag is set.
		/// </summary>
		/// <param name="invocationTypes">The invocation types.</param>
		/// <returns></returns>
		public static bool IsMoreThanOneInvocationType(this InvocationTypes invocationTypes)
		{
			return (invocationTypes & (invocationTypes - 1)) != 0; // is not a power of 2
		}

		/// <summary>
		/// Gets the collection of generic types associated with a set of invocation types.
		/// </summary>
		/// <param name="invocationTypes">The invocation types.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetGenericTypesAssociatedWithInvocationTypes(this InvocationTypes invocationTypes)
		{
			if (invocationTypes.HasFlag(InvocationTypes.Query))
				yield return typeof(IQueryHandler<,>);
			if (invocationTypes.HasFlag(InvocationTypes.AsyncQuery))
				yield return typeof(IAsyncQueryHandler<,>);
			if (invocationTypes.HasFlag(InvocationTypes.Command))
				yield return typeof(ICommandHandler<>);
			if (invocationTypes.HasFlag(InvocationTypes.AsyncCommand))
				yield return typeof(IAsyncCommandHandler<>);
			if (invocationTypes.HasFlag(InvocationTypes.ResultCommand))
				yield return typeof(IResultCommandHandler<,>);
			if (invocationTypes.HasFlag(InvocationTypes.AsyncResultCommand))
				yield return typeof(IAsyncResultCommandHandler<,>);
		}
	}
}
