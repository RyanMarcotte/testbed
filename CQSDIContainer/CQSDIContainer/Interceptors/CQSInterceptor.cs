using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	/// <summary>
	/// Base class for interceptors applied to CQS handlers.
	/// </summary>
	public abstract class CQSInterceptor : IInterceptor
	{
		/// <summary>
		/// Intercept a handler invocation and wrap some cross-cutting concern around it.
		/// </summary>
		/// <param name="invocation">The handler invocation being intercepted.</param>
		public void Intercept(IInvocation invocation)
		{
			if (IsAsyncMethod(invocation.Method))
				InterceptAsync(invocation);
			else
				InterceptSync(invocation);
		}

		/// <summary>
		/// Interception logic for synchronous handlers.
		/// </summary>
		/// <param name="invocation"></param>
		protected abstract void InterceptSync(IInvocation invocation);

		/// <summary>
		/// Interception logic for asynchronous handlers.
		/// </summary>
		/// <param name="invocation"></param>
		protected abstract void InterceptAsync(IInvocation invocation);

		/// <summary>
		/// Determines if a method is synchronous or asynchronous.
		/// </summary>
		/// <param name="method">The method info.</param>
		/// <returns></returns>
		private static bool IsAsyncMethod(MethodInfo method)
		{
			return method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
		}
	}	
}
