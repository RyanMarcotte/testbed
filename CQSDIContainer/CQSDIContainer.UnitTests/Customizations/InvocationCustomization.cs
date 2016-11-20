using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using CQSDIContainer.UnitTests.Interceptors;
using FakeItEasy;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests.Customizations
{
	/// <summary>
	/// Indicates the type of method call represented by a fake <see cref="IInvocation"/> object.
	/// </summary>
	public enum InvocationMethodType
	{
		Synchronous,
		AsynchronousAction,
		AsynchronousFunction
	}

	/// <summary>
	/// Exception thrown by a fake <see cref="IInvocation"/> object.
	/// </summary>
	public class InvocationFailedException : Exception
	{

	}

	/// <summary>
	/// Customization that registers a fake for <see cref="IInvocation"/> objects.
	/// </summary>
	public class InvocationCustomization : ICustomization
	{
		private readonly bool _invocationCompletesSuccessfully;
		private readonly InvocationMethodType _invocationMethodType;

		public InvocationCustomization(bool invocationCompletesSuccessfully, InvocationMethodType invocationMethodType)
		{
			_invocationCompletesSuccessfully = invocationCompletesSuccessfully;
			_invocationMethodType = invocationMethodType;
		}

		public void Customize(IFixture fixture)
		{
			var invocation = A.Fake<IInvocation>();

			var methodInfo = GetMethodInfoForType(_invocationMethodType);
			A.CallTo(() => invocation.ToString()).Returns(_invocationMethodType.ToString());
			A.CallTo(() => invocation.Method).Returns(methodInfo);

			switch (_invocationMethodType)
			{
				case InvocationMethodType.Synchronous:
					A.CallTo(() => invocation.ReturnValue).Returns(typeof(void));
					break;
				case InvocationMethodType.AsynchronousAction:
					A.CallTo(() => invocation.ReturnValue).Returns(AsynchronousAction());
					break;
				case InvocationMethodType.AsynchronousFunction:
					A.CallTo(() => invocation.ReturnValue).Returns(AsynchronousFunction(new object()));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (_invocationCompletesSuccessfully)
				A.CallTo(() => invocation.Proceed()).DoesNothing();
			else
				A.CallTo(() => invocation.Proceed()).Throws<InvocationFailedException>();

			fixture.Register(() => invocation);
		}

		private static MethodInfo GetMethodInfoForType(InvocationMethodType invocationMethodType)
		{
			switch (invocationMethodType)
			{
				case InvocationMethodType.Synchronous:
					return typeof(InvocationCustomization).GetMethod(nameof(SynchronousMethod), BindingFlags.Static | BindingFlags.NonPublic);

				case InvocationMethodType.AsynchronousAction:
					return typeof(InvocationCustomization).GetMethod(nameof(AsynchronousAction), BindingFlags.Static | BindingFlags.NonPublic);

				case InvocationMethodType.AsynchronousFunction:
					return typeof(InvocationCustomization).GetMethod(nameof(AsynchronousFunction), BindingFlags.Static | BindingFlags.NonPublic);

				default:
					throw new ArgumentOutOfRangeException(nameof(invocationMethodType), invocationMethodType, null);
			}
		}

		private static void SynchronousMethod()
		{
			// do nothing
		}

		private static async Task AsynchronousAction()
		{
			await new Task(() => { });
		}

		private static async Task<object> AsynchronousFunction(object value)
		{
			return await new Task<object>(() => value);
		}
	}
}
