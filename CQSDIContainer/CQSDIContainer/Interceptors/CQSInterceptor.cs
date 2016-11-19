using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public abstract class CQSInterceptor : IInterceptor
	{
		protected abstract void InterceptSync(IInvocation invocation);
		protected abstract void InterceptAsync(IInvocation invocation);
		
		public void Intercept(IInvocation invocation)
		{
			if (IsAsyncMethod(invocation.Method))
				InterceptAsync(invocation);
			else
				InterceptSync(invocation);
		}

		private static bool IsAsyncMethod(MethodInfo method)
		{
			return method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
		}
	}	
}
