using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DoubleCache;
using DoubleCache.LocalCache;

namespace IQ.CQS.IoC.Installers
{
	public class CacheInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<ICacheAside>().UsingFactoryMethod(() => new MemCache()).LifestyleSingleton());
		}
	}
}
