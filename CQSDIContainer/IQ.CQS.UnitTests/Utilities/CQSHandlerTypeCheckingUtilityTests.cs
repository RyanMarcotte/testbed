using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IQ.CQS.Utilities;
using IQ.Vanilla;
using IQ.Vanilla.CQS;
using Xunit;

// ReSharper disable ClassNeverInstantiated.Local

namespace IQ.CQS.UnitTests.Utilities
{
	/// <summary>
	/// Unit tests for the <see cref="CQSHandlerTypeCheckingUtility"/> class.
	/// </summary>
	public class CQSHandlerTypeCheckingUtilityTests
	{
		public class WhenDeterminingIfObjectIsQueryHandler
		{
			[Theory]
			[InlineData(typeof(IQueryHandler<SampleQuery, int>))]
			[InlineData(typeof(SampleQueryHandler))]
			[InlineData(typeof(IAsyncQueryHandler<SampleQuery, int>))]
			[InlineData(typeof(SampleAsyncQueryHandler))]
			public void ShouldReturnTrueIfTypeIsQueryHandler(Type type)
			{
				CQSHandlerTypeCheckingUtility.IsQueryHandler(type).Should().BeTrue();
			}

			[Theory]
			[InlineData(typeof(ICommandHandler<SampleCommand>))]
			[InlineData(typeof(SampleCommandHandler))]
			[InlineData(typeof(IAsyncCommandHandler<SampleCommand>))]
			[InlineData(typeof(SampleAsyncCommandHandler))]
			[InlineData(typeof(IResultCommandHandler<SampleCommand, int>))]
			[InlineData(typeof(SampleResultCommandHandler))]
			[InlineData(typeof(IAsyncResultCommandHandler<SampleCommand, int>))]
			[InlineData(typeof(SampleAsyncResultCommandHandler))]
			public void ShouldReturnFalseIfTypeIsNotQueryHandler(Type type)
			{
				CQSHandlerTypeCheckingUtility.IsQueryHandler(type).Should().BeFalse();
			}
		}

		public class WhenDeterminingIfObjectIsCommandHandler
		{
			[Theory]
			[InlineData(typeof(ICommandHandler<SampleCommand>))]
			[InlineData(typeof(SampleCommandHandler))]
			[InlineData(typeof(IAsyncCommandHandler<SampleCommand>))]
			[InlineData(typeof(SampleAsyncCommandHandler))]
			[InlineData(typeof(IResultCommandHandler<SampleCommand, int>))]
			[InlineData(typeof(SampleResultCommandHandler))]
			[InlineData(typeof(IAsyncResultCommandHandler<SampleCommand, int>))]
			[InlineData(typeof(SampleAsyncResultCommandHandler))]
			public void ShouldReturnTrueIfTypeIsCommandHandler(Type type)
			{
				CQSHandlerTypeCheckingUtility.IsCommandHandler(type).Should().BeTrue();
			}

			[Theory]
			[InlineData(typeof(IQueryHandler<SampleQuery, int>))]
			[InlineData(typeof(SampleQueryHandler))]
			[InlineData(typeof(IAsyncQueryHandler<SampleQuery, int>))]
			[InlineData(typeof(SampleAsyncQueryHandler))]
			public void ShouldReturnFalseIfTypeIsNotCommandHandler(Type type)
			{
				CQSHandlerTypeCheckingUtility.IsCommandHandler(type).Should().BeFalse();
			}
		}

		public class WhenDeterminingIfObjectIsCQSHandler
		{
			[Theory]
			[InlineData(typeof(ICommandHandler<SampleCommand>))]
			[InlineData(typeof(SampleCommandHandler))]
			[InlineData(typeof(IAsyncCommandHandler<SampleCommand>))]
			[InlineData(typeof(SampleAsyncCommandHandler))]
			[InlineData(typeof(IResultCommandHandler<SampleCommand, int>))]
			[InlineData(typeof(SampleResultCommandHandler))]
			[InlineData(typeof(IAsyncResultCommandHandler<SampleCommand, int>))]
			[InlineData(typeof(SampleAsyncResultCommandHandler))]
			[InlineData(typeof(IQueryHandler<SampleQuery, int>))]
			[InlineData(typeof(SampleQueryHandler))]
			[InlineData(typeof(IAsyncQueryHandler<SampleQuery, int>))]
			[InlineData(typeof(SampleAsyncQueryHandler))]
			public void ShouldReturnTrueIfTypeIsCommandHandler(Type type)
			{
				CQSHandlerTypeCheckingUtility.IsCQSHandler(type).Should().BeTrue();
			}

			[Theory]
			[InlineData(typeof(object))]
			public void ShouldReturnFalseIfTypeIsNotCommandHandler(Type type)
			{
				CQSHandlerTypeCheckingUtility.IsCQSHandler(type).Should().BeFalse();
			}
		}

		#region Query Handlers

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
				return await new Task<int>(() => 5);
			}
		}

		#endregion

		#region Command Handlers

		private class SampleCommand
		{
			
		}

		private class SampleCommandHandler : ICommandHandler<SampleCommand>
		{
			public void Handle(SampleCommand command)
			{
				
			}
		}

		private class SampleAsyncCommandHandler : IAsyncCommandHandler<SampleCommand>
		{
			public async Task HandleAsync(SampleCommand command, CancellationToken cancellationToken = new CancellationToken())
			{
				await new Task(() => { });
			}
		}

		private class SampleResultCommandHandler : IResultCommandHandler<SampleCommand, int>
		{
			public Result<Unit, int> Handle(SampleCommand command)
			{
				return Result.Succeed<Unit, int>(Unit.Value);
			}
		}

		private class SampleAsyncResultCommandHandler : IAsyncResultCommandHandler<SampleCommand, int>
		{
			public async Task<Result<Unit, int>> HandleAsync(SampleCommand command, CancellationToken cancellationToken)
			{
				return await new Task<Result<Unit, int>>(() => Result.Succeed<Unit, int>(Unit.Value), cancellationToken);
			}
		}

		#endregion
	}
}
