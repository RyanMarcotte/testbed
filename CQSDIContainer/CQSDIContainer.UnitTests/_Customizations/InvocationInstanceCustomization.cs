using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests.TestUtilities;
using Ploeh.AutoFixture;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Customizations
{
	public class InvocationInstanceCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var invocation = CQSInvocationCustomization.BuildInvocation(true, CQSHandlerType.Query);
			var componentModel = ComponentModelCustomization.BuildComponentModel(SampleHandlerFactory.GetCQSHandlerComponentModelTypeFromHandlerType(CQSHandlerType.Query));

			fixture.Register(() => BuildInvocationInstance(invocation, componentModel));
		}

		public static InvocationInstance BuildInvocationInstance(IInvocation invocation, ComponentModel componentModel)
		{
			return new InvocationInstance(invocation, componentModel);
		}
	}
}
