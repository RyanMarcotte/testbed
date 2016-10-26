using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.QueryDecorators.Interfaces;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.QueryDecorators
{
	public class LogExecutionTimeToConsoleQueryHandlerDecorator<TQuery, TResult> : IDecorateQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		private readonly IQueryHandler<TQuery, TResult> _queryHandler;

		public LogExecutionTimeToConsoleQueryHandlerDecorator(IQueryHandler<TQuery, TResult> queryHandler)
		{
			_queryHandler = queryHandler;
		}

		public TResult Handle(TQuery command)
		{
			var begin = DateTime.UtcNow;
			var result = _queryHandler.Handle(command);
			var end = DateTime.UtcNow;
			Console.WriteLine($"{_queryHandler.GetType()} measured time: {(end - begin).TotalMilliseconds} ms");

			return result;
		}
	}
}
