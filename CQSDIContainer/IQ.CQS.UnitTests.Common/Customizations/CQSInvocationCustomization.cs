using Castle.DynamicProxy;
using FakeItEasy;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Exceptions;
using IQ.CQS.UnitTests.Framework.Utilities;
using Ploeh.AutoFixture;

namespace IQ.CQS.UnitTests.Framework.Customizations
{
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
			A.CallTo(() => invocation.InvocationTarget).Returns(SampleCQSHandlerImplementationFactory.GetNewHandlerInstanceForHandlerType(handlerType));
			A.CallTo(() => invocation.Method).Returns(SampleCQSHandlerImplementationFactory.GetMethodInfoFromHandlerType(handlerType));
			A.CallTo(() => invocation.Arguments).Returns(SampleCQSHandlerImplementationFactory.GetArgumentsUsedForHandleAndHandleAsyncMethodsForHandlerType(handlerType));
			A.CallTo(() => invocation.ReturnValue).Returns(SampleCQSHandlerImplementationFactory.GetReturnValueForHandlerType(handlerType));
			
			if (completesSuccessfully)
				A.CallTo(() => invocation.Proceed()).DoesNothing();
			else
				A.CallTo(() => invocation.Proceed()).Throws<InvocationFailedException>();

			return invocation;
		}
	}
}
