using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests.Customizations;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.TestUtilities
{
	public static class ComponentModelFactory
	{
		public static Type GetQueryHandlerComponentModelTypeFromMethodType(InvocationMethodType methodType)
		{
			switch (methodType)
			{
				case InvocationMethodType.SynchronousFunction:
					return typeof(SampleQueryHandler);
				case InvocationMethodType.AsynchronousFunction:
					return typeof(SampleAsyncQueryHandler);
				default:
					throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
			}
		}

		public static Type GetCommandHandlerComponentModelTypeFromMethodType(InvocationMethodType methodType)
		{
			switch (methodType)
			{
				case InvocationMethodType.SynchronousAction:
					return typeof(SampleCommandHandler);
				case InvocationMethodType.SynchronousFunction:
					return typeof(SampleResultCommandHandler);
				case InvocationMethodType.AsynchronousAction:
					return typeof(SampleAsyncCommandHandler);
				case InvocationMethodType.AsynchronousFunction:
					return typeof(SampleAsyncCommandHandlerWithResult);
				default:
					throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
			}
		}

		private class SampleQuery : IQuery<int>
		{
			public SampleQuery(int value)
			{
				Value = value;
			}

			public int Value { get; }	
		}

		private class SampleQueryHandler : IQueryHandler<SampleQuery, int>
		{
			public int Handle(SampleQuery query)
			{
				return query.Value;
			}
		}

		private class SampleAsyncQueryHandler : IAsyncQueryHandler<SampleQuery, int>
		{
			public async Task<int> HandleAsync(SampleQuery query, CancellationToken cancellationToken = new CancellationToken())
			{
				return await new Task<int>(() => query.Value, cancellationToken);
			}
		}

		private class SampleCommandHandler : ICommandHandler<int>
		{
			public void Handle(int command)
			{
			}
		}

		private class SampleResultCommandHandler : IResultCommandHandler<int, int>
		{
			public Result<Unit, int> Handle(int command)
			{
				return Result.Succeed<Unit, int>(Unit.Value);
			}
		}
		private class SampleAsyncCommandHandler : IAsyncCommandHandler<int>
		{
			public async Task HandleAsync(int command, CancellationToken cancellationToken = new CancellationToken())
			{
				await new Task(() => { });
			}
		}

		private class SampleAsyncCommandHandlerWithResult : IAsyncResultCommandHandler<int, int>
		{
			public async Task<Result<Unit, int>> HandleAsync(int command, CancellationToken cancellationToken)
			{
				return await new Task<Result<Unit, int>>(() => Result.Succeed<Unit, int>(Unit.Value));
			}
		}
	}
}
