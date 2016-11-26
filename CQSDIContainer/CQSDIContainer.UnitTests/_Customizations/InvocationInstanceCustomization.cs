using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests._TestUtilities;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests._Customizations
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
