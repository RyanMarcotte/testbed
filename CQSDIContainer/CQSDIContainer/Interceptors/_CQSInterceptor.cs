using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Interceptors
{
	/// <summary>
	/// Base class for interceptors applied to CQS handlers.
	/// </summary>
	/// <remarks>
	/// http://www.codeproject.com/Articles/1080517/Aspect-Oriented-Programming-using-Interceptors-wit
	/// http://stackoverflow.com/questions/28099669/intercept-async-method-that-returns-generic-task-via-dynamicproxy
	/// </remarks>
	public abstract class CQSInterceptor : IInterceptor, IOnBehalfAware
	{
		private ComponentModel _componentModel;

		/// <summary>
		/// Intercept a handler invocation and wrap some cross-cutting concern around it.
		/// </summary>
		/// <param name="invocation">The handler invocation being intercepted.</param>
		public void Intercept(IInvocation invocation)
		{
			if (!CQSHandlerTypeCheckingUtility.IsCQSHandler(_componentModel.Implementation))
				throw new InvalidOperationException("A CQS interceptor may only intercept CQS handlers!!");

			var methodType = GetMethodType(invocation.Method);
			if (!ApplyToNestedHandlers) // and some kind of check to see if we're running a nested handler
			{
				// proceed with invocation without running through interceptor
				invocation.Proceed();
				switch (methodType)
				{
					case MethodType.AsynchronousAction:
						invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue);
						break;

					case MethodType.AsynchronousFunction:
						ExecuteHandleAsyncWithResultUsingReflection(invocation);
						break;

					case MethodType.Synchronous:
						break;

					default:
						throw new ArgumentOutOfRangeException($"Invalid method type '{methodType}'!!");
				}
			}
			else
			{
				// we're not intercepting the outermost handler's Handle method, so intercept the invocation
				if (methodType == MethodType.AsynchronousAction || methodType == MethodType.AsynchronousFunction)
					InterceptAsync(invocation, _componentModel, methodType == MethodType.AsynchronousAction ? AsynchronousMethodType.Action : AsynchronousMethodType.Function);
				else
					InterceptSync(invocation, _componentModel);
			}
		}

		/// <summary>
		/// Cache the component model associated with the intercepted invocation.
		/// </summary>
		/// <param name="target">The component model.</param>
		public void SetInterceptedComponentModel(ComponentModel target)
		{
			_componentModel = target;
		}

		/// <summary>
		/// Indicates if the interceptor should be applied to nested handlers.
		/// </summary>
		protected abstract bool ApplyToNestedHandlers { get; }

		/// <summary>
		/// Interception logic for synchronous handlers.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected abstract void InterceptSync(IInvocation invocation, ComponentModel componentModel);

		/// <summary>
		/// Interception logic for asynchronous handlers.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="methodType">The asynchronous method type.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected abstract void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType);

		#region Enums

		/// <summary>
		/// Indicates the type of method.
		/// </summary>
		private enum MethodType
		{
			/// <summary>
			/// Does not return Task or Task&lt;T>.
			/// </summary>
			Synchronous,

			/// <summary>
			/// Returns Task.
			/// </summary>
			AsynchronousAction,

			/// <summary>
			/// Returns Task&lt;T>.
			/// </summary>
			AsynchronousFunction
		}

		/// <summary>
		/// Indicates the type of asynchronous method.
		/// </summary>
		protected enum AsynchronousMethodType
		{
			/// <summary>
			/// The asynchronous method returns Task.
			/// </summary>
			Action,

			/// <summary>
			/// The asynchronous method returns Task&lt;T>.
			/// </summary>
			Function
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Determines if a method is synchronous or asynchronous.
		/// </summary>
		/// <param name="method">The method info.</param>
		/// <returns></returns>
		private static MethodType GetMethodType(MethodInfo method)
		{
			var returnType = method.ReturnType;
			if (returnType == typeof(Task))
				return MethodType.AsynchronousAction;
			if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
				return MethodType.AsynchronousFunction;

			return MethodType.Synchronous;
		}

		private static async Task HandleAsync(Task task)
		{
			await task;
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task)
		{
			return await task;
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncWithResultMethodInfo = typeof(CQSInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private static void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericMethodLookup.GetOrAdd(resultType, _handleAsyncWithResultMethodInfo.MakeGenericMethod(resultType));
			invocation.ReturnValue = methodInfo.Invoke(null, new[] { invocation.ReturnValue });
		}

		#endregion
	}
}
