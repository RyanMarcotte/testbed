using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace IQ.CQS.Lab.Installers
{
	public class MockRQApplicationServiceInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IRQApplicationServiceMock>().ImplementedBy<MockRQApplicationServiceUsingDI>().LifestyleTransient());
		}
	}
}
