using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DoubleCache;
using DoubleCache.LocalCache;

namespace CQSDIContainer.Installers
{
	public class CacheInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<ICacheAside>().UsingFactoryMethod(() => new MemCache()).LifestyleSingleton());
		}
	}
}
