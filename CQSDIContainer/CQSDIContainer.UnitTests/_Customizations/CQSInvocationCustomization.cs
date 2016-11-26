using System;
using Castle.DynamicProxy;
using CQSDIContainer.UnitTests._TestUtilities;
using FakeItEasy;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests._Customizations
{
	/// <summary>
	/// Exception thrown by a fake <see cref="IInvocation"/> object.
	/// </summary>
	public class InvocationFailedException : Exception
	{

	}

	/// <summary>
	/// Customization that registers a fake for <see cref="IInvocation"/> objects.
	/// </summary>
	public class CQSInvocationCustomization : ICustomization
	{
		private readonly bool _invocationCompletesSuccessfully;
		private readonly CQSHandlerType _cqsHandlerType;

		public CQSInvocationCustomization()
		{
			_invocationCompletesSuccessfully = false;
			_cqsHandlerType = CQSHandlerType.Command;
		}

		public CQSInvocationCustomization(bool invocationCompletesSuccessfully, CQSHandlerType cqsHandlerType)
		{
			_invocationCompletesSuccessfully = invocationCompletesSuccessfully;
			_cqsHandlerType = cqsHandlerType;
		}
		
		public void Customize(IFixture fixture)
		{
			fixture.Register(() => BuildInvocation(_invocationCompletesSuccessfully, _cqsHandlerType));
		}

		public static IInvocation BuildInvocation(bool completesSuccessfully, CQSHandlerType handlerType)
		{
			var invocation = A.Fake<IInvocation>();

			A.CallTo(() => invocation.ToString()).Returns(handlerType.ToString());
			A.CallTo(() => invocation.InvocationTarget).Returns(SampleHandlerFactory.GetHandlerInstanceForHandlerType(handlerType));
			A.CallTo(() => invocation.Method).Returns(SampleHandlerFactory.GetMethodInfoFromHandlerType(handlerType));
			A.CallTo(() => invocation.Arguments).Returns(SampleHandlerFactory.GetArgumentsForHandlerType(handlerType));
			A.CallTo(() => invocation.ReturnValue).Returns(SampleHandlerFactory.GetReturnValueForHandlerType(handlerType));
			
			if (completesSuccessfully)
				A.CallTo(() => invocation.Proceed()).DoesNothing();
			else
				A.CallTo(() => invocation.Proceed()).Throws<InvocationFailedException>();

			return invocation;
		}
	}
}
