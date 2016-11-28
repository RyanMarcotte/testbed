using Castle.Core;
using Castle.DynamicProxy;
using IQ.CQS.Interceptors;
using IQ.CQS.UnitTests.Framework.Customizations;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Utilities;
using Ploeh.AutoFixture;

namespace IQ.CQS.UnitTests._Customizations
{
	public class InvocationInstanceCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var invocation = CQSInvocationCustomization.BuildInvocation(true, CQSHandlerType.Query_ReturnsValueType);
			var componentModel = ComponentModelCustomization.BuildComponentModel(SampleCQSHandlerImplementationFactory.GetSampleImplementationClassTypeForHandlerType(CQSHandlerType.Query_ReturnsValueType));

			fixture.Register(() => BuildInvocationInstance(invocation, componentModel));
		}

		public static InvocationInstance BuildInvocationInstance(IInvocation invocation, ComponentModel componentModel)
		{
			return new InvocationInstance(invocation, componentModel);
		}
	}
}
