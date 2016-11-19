﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace CQSDIContainer.Installers
{
	public class MockRQApplicationServiceInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IRQApplicationServiceMock>().ImplementedBy<MockRQApplicationServiceUsingDI>().LifestyleTransient());
		}
	}
}
