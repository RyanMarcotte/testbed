using System;
using CQSDIContainer.Commands;
using CQSDIContainer.Queries;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer
{
	internal class MockRQApplicationServiceUsingCQSFactory
	{
		private readonly IQueryHandler<GetIntegerQuery, int> _getIntegerQueryHandler;
		private readonly ICommandHandler<DoNothingAndDoSomethingCommand> _doNothingAndDoSomethingCommandHandler;
		private readonly ICommandHandler<ZDoNothingAndDoSomethingCommand> _zDoNothingAndDoSomethingCommandHandler;
		private readonly IQueryHandler<GetTupleQuery, Tuple<int, string, int>> _getTupleQueryHandler;

		public MockRQApplicationServiceUsingCQSFactory(ICQSFactory cqsFactory)
		{
			_getIntegerQueryHandler = cqsFactory.CreateQueryHandler<GetIntegerQuery, int>();
			_doNothingAndDoSomethingCommandHandler = cqsFactory.CreateCommandHandler<DoNothingAndDoSomethingCommand>();
			_zDoNothingAndDoSomethingCommandHandler = cqsFactory.CreateCommandHandler<ZDoNothingAndDoSomethingCommand>();
			_getTupleQueryHandler = cqsFactory.CreateQueryHandler<GetTupleQuery, Tuple<int, string, int>>();
		}

		public void DoStuff()
		{
			int id = _getIntegerQueryHandler.Handle(new GetIntegerQuery(10));
			int id2 = _getIntegerQueryHandler.Handle(new GetIntegerQuery(10));
			int id3 = _getIntegerQueryHandler.Handle(new GetIntegerQuery(10));
			_doNothingAndDoSomethingCommandHandler.Handle(new DoNothingAndDoSomethingCommand(id));
			_zDoNothingAndDoSomethingCommandHandler.Handle(new ZDoNothingAndDoSomethingCommand(id));
			Console.ReadLine();

			// -- CACHE DEMO --
			// the first call will populate the cache; subsequent calls will use the cache item; output of below is...
			// getting value (maybe from cache)
			// executing query to retrieve tuple
			// getting value (maybe from cache)
			// getting value (maybe from cache)
			// getting value (maybe from cache)
			// executing query to retrieve tuple
			// getting value (maybe from cache)
			_getTupleQueryHandler.Handle(new GetTupleQuery(1337, 666));
			_getTupleQueryHandler.Handle(new GetTupleQuery(1337, 666));
			_getTupleQueryHandler.Handle(new GetTupleQuery(1337, 666));
			_getTupleQueryHandler.Handle(new GetTupleQuery(15, 225));
			_getTupleQueryHandler.Handle(new GetTupleQuery(15, 225));
			Console.ReadLine();
		}
	}
}