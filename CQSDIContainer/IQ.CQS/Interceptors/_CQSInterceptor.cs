using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using IQ.CQS.Utilities;

namespace IQ.CQS.Interceptors
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

			Console.WriteLine($"<< begin {GetType()} intercept >>");
			
			var methodType = GetMethodType(invocation.Method);
			if (methodType == MethodType.AsynchronousAction || methodType == MethodType.AsynchronousFunction)
				InterceptAsync(invocation, _componentModel, methodType == MethodType.AsynchronousAction ? AsynchronousMethodType.Action : AsynchronousMethodType.Function);
			else
				InterceptSync(invocation, _componentModel);

			Console.WriteLine($"<< end {GetType()} intercept >>");
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

		#endregion
	}
}
