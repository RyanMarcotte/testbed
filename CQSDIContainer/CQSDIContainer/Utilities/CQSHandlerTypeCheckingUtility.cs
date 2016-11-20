
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Utilities
{
	/// <summary>
	/// Utility class for performing CQS handler class type-checks.
	/// </summary>
	public static class CQSHandlerTypeCheckingUtility
	{
		/// <summary>
		/// Indicates if the specified type corresponds to a query handler.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static bool IsQueryHandler(Type type)
		{
			if (type.IsInterface)
				return IsQueryHandlerInterface(type);
			
			return type.IsClass && type.GetInterfaces().Any(IsQueryHandlerInterface);
		}

		/// <summary>
		/// Indicates if the specified type corresponds to a command handler.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static bool IsCommandHandler(Type type)
		{
			if (type.IsInterface)
				return IsCommandHandlerInterface(type);

			return type.IsClass && type.GetInterfaces().Any(IsCommandHandlerInterface);
		}

		/// <summary>
		/// Indicates if the specified type corresponds to a query or command handler.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static bool IsCQSHandler(Type type)
		{
			return IsQueryHandler(type) || IsCommandHandler(type);
		}

		#region Internals

		private static readonly IEnumerable<Type> _queryHandlerTypes = new HashSet<Type>
			{
				typeof(IQueryHandler<,>),
				typeof(IAsyncQueryHandler<,>)
			};

		private static readonly IEnumerable<Type> _commandHandlerTypes = new HashSet<Type>
			{
				typeof(ICommandHandler<>),
				typeof(IAsyncCommandHandler<>),
				typeof(IResultCommandHandler<,>),
				typeof(IAsyncResultCommandHandler<,>)
			};

		private static bool IsQueryHandlerInterface(Type type)
		{
			return type.IsGenericType && _queryHandlerTypes.Contains(type.GetGenericTypeDefinition());
		}

		private static bool IsCommandHandlerInterface(Type type)
		{
			return type.IsGenericType && _commandHandlerTypes.Contains(type.GetGenericTypeDefinition());
		}

		#endregion
	}
}
