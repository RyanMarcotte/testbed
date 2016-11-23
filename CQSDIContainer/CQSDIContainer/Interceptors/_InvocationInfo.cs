using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;

namespace CQSDIContainer.Interceptors
{
	public class InvocationInstance : IEquatable<InvocationInstance>
	{
		private readonly MethodInfo _invocationMethodInfo;
		private readonly Type _componentModelType;
		private readonly int _hashCode;

		public InvocationInstance(IInvocation invocation, ComponentModel componentModel)
		{
			_invocationMethodInfo = invocation.Method;
			_componentModelType = componentModel.Implementation;
			_hashCode = invocation.GetHashCode() ^ componentModel.GetHashCode();
		}

		public bool Equals(InvocationInstance other)
		{
			if (other == null)
				return false;

			return _invocationMethodInfo == other._invocationMethodInfo && _componentModelType == other._componentModelType;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var otherInstance = obj as InvocationInstance;
			return otherInstance != null && Equals(otherInstance);
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}
	}
}
