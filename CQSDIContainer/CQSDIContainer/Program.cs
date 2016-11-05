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
			container.Install(new CQSInstaller());

			Console.WriteLine("Finished installing");

			new ProgramLogic(new CQSFactory(container)).DoStuff();
		}
	}

	internal class ProgramLogic
	{
		private readonly IQueryHandler<GetIntegerQuery, int> _getIntegerQueryHandler;
		private readonly ICommandHandler<DoNothingAndDoSomethingCommand> _doNothingAndDoSomethingCommandHandler;
		private readonly ICommandHandler<ZDoNothingAndDoSomethingCommand> _zDoNothingAndDoSomethingCommandHandler;

		public ProgramLogic(ICQSFactory cqsFactory)
		{
			_getIntegerQueryHandler = cqsFactory.CreateQueryHandler<GetIntegerQuery, int>();
			_doNothingAndDoSomethingCommandHandler = cqsFactory.CreateCommandHandler<DoNothingAndDoSomethingCommand>();
			_zDoNothingAndDoSomethingCommandHandler = cqsFactory.CreateCommandHandler<ZDoNothingAndDoSomethingCommand>();
		}

		public void DoStuff()
		{
			int id = _getIntegerQueryHandler.Handle(new GetIntegerQuery());
			_doNothingAndDoSomethingCommandHandler.Handle(new DoNothingAndDoSomethingCommand(id));
			_zDoNothingAndDoSomethingCommandHandler.Handle(new ZDoNothingAndDoSomethingCommand(id));
			Console.ReadLine();
		}
	}
}
