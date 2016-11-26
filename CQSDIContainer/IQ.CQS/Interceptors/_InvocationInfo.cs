using System;
using System.Reflection;
using Castle.Core;
using Castle.DynamicProxy;

namespace IQ.CQS.Interceptors
{
	public class InvocationInstance : IEquatable<InvocationInstance>
	{
		private readonly MethodInfo _invocationMethodInfo;
		private readonly int _hashCode;

		public InvocationInstance(IInvocation invocation, ComponentModel componentModel)
		{
			_invocationMethodInfo = invocation.Method;
			_hashCode = invocation.GetHashCode() ^ componentModel.GetHashCode();

			ComponentModelImplementationType = componentModel.Implementation;
		}
		
		public Type ComponentModelImplementationType { get; }
		public string MethodName => _invocationMethodInfo.Name;

		public bool Equals(InvocationInstance other)
		{
			if (other == null)
				return false;

			return _invocationMethodInfo == other._invocationMethodInfo && ComponentModelImplementationType == other.ComponentModelImplementationType;
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
