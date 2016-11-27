using System;
using System.Reflection;
using Castle.Core;
using Castle.DynamicProxy;

namespace IQ.CQS.Interceptors
{
	/// <summary>
	/// Encapsulates a CQS handler's Handle / HandleAsync unique invocation instance.
	/// </summary>
	public class InvocationInstance : IEquatable<InvocationInstance>
	{
		private readonly MethodInfo _invocationMethodInfo;
		private readonly int _hashCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="InvocationInstance"/> class.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model.</param>
		public InvocationInstance(IInvocation invocation, ComponentModel componentModel)
		{
			_invocationMethodInfo = invocation.Method;
			ParameterObject = invocation.Arguments[0];
			_hashCode = invocation.GetHashCode() ^ componentModel.GetHashCode();

			ComponentModelImplementationType = componentModel.Implementation;
		}
		
		/// <summary>
		/// Gets the type of the component model associated with the invocation.
		/// </summary>
		public Type ComponentModelImplementationType { get; }
		
		/// <summary>
		/// Gets the invocation's parameter object.
		/// </summary>
		public object ParameterObject { get; }

		/// <summary>
		/// Gets the name of the method name of the invocation.
		/// </summary>
		public string MethodName => _invocationMethodInfo.Name;

		/// <summary>
		/// Compare an <see cref="InvocationInstance"/> against another <see cref="InvocationInstance"/> for equality.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(InvocationInstance other)
		{
			if (other == null)
				return false;

			return _invocationMethodInfo == other._invocationMethodInfo && ComponentModelImplementationType == other.ComponentModelImplementationType;
		}

		/// <summary>
		/// Compare the <see cref="InvocationInstance"/> against another <see cref="object"/> for equality.
		/// </summary>
		/// <param name="obj">The other object.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var otherInstance = obj as InvocationInstance;
			return otherInstance != null && Equals(otherInstance);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _hashCode;
		}
	}
}
