using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace IQ.CQS.Lab
{
	internal class Program
	{
		// http://tommarien.github.io/blog/2013/05/11/i-command-you/
		private static void Main(string[] args)
		{
			var container = new WindsorContainer();
			container.AddFacility<TypedFactoryFacility>();
			container.Install(FromAssembly.This());

			var service = container.Resolve<IRQApplicationServiceMock>();
			service.DoStuff().GetAwaiter().GetResult();
		}
	}
}
