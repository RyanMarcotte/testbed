using System;
using System.Collections.Generic;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Interceptors.Enums
{
	/// <summary>
	/// Enumeration of CQS handler invocation types.
	/// </summary>
	[Flags]
	public enum InvocationTypes
	{
		/// <summary>
		/// Not a CQS handler invocation type.
		/// </summary>
		None = 0,
		
		/// <summary>
		/// Is a <see cref="IQueryHandler{TQuery,TResult}"/>.
		/// </summary>
		Query = 1,

		/// <summary>
		/// Is a <see cref="IAsyncQueryHandler{TQuery, TResult}"/>.
		/// </summary>
		AsyncQuery = 2,

		/// <summary>
		/// Is a <see cref="ICommandHandler{TCommand}"/>.
		/// </summary>
		Command = 4,

		/// <summary>
		/// Is a <see cref="IAsyncCommandHandler{TCommand}"/>.
		/// </summary>
		AsyncCommand = 8,

		/// <summary>
		/// Is a <see cref="IResultCommandHandler{TCommand, TError}"/>.
		/// </summary>
		ResultCommand = 16,
		
		/// <summary>
		/// Is a <see cref="IAsyncResultCommandHandler{TCommand, TError}"/>.
		/// </summary>
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
