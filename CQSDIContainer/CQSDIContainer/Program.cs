using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using CQSDIContainer.Commands;
using CQSDIContainer.Installers;
using CQSDIContainer.Queries;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer
{
	internal class Program
	{
		// http://tommarien.github.io/blog/2013/05/11/i-command-you/
		private static void Main(string[] args)
		{
			var container = new WindsorContainer();
			container.AddFacility<TypedFactoryFacility>();
			container.Install(new CacheInstaller());
			container.Install(new CQSInstaller());

			Console.WriteLine("Finished installing");

			//new ProgramLogicForCQSFactoryDemo(new CQSFactory(container)).DoStuff();
			new MockRQApplicationServiceUsingDI(
				container.Resolve<IQueryHandler<GetIntegerQuery, int>>(),
				container.Resolve<IAsyncQueryHandler<GetStringAsyncQuery, string>>(),
				container.Resolve<ICommandHandler<DoNothingAndDoSomethingCommand>>(),
				container.Resolve<IAsyncCommandHandler<DoSomethingAsyncCommand>>(),
				container.Resolve<IResultCommandHandler<DoSomethingWithResultCommand, DoSomethingWithResultCommandHandlerErrorCode>>(),
				container.Resolve<IAsyncResultCommandHandler<DoSomethingAsyncWithResultCommand, DoSomethingAsyncWithResultCommandHandlerErrorCode>>()
			).DoStuff().GetAwaiter().GetResult();
		}
	}
}
