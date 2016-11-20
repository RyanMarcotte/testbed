using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQSDIContainer.UnitTests.Customizations;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests.Interceptors
{
	public abstract class CQSInterceptorTestsBase
	{
		protected static Type GetComponentModelTypeFromMethodType(InvocationMethodType methodType)
		{
			switch (methodType)
			{
				case InvocationMethodType.Synchronous:
					return typeof(SampleHandler);
				case InvocationMethodType.AsynchronousAction:
					return typeof(SampleAsyncHandler);
				case InvocationMethodType.AsynchronousFunction:
					return typeof(SampleAsyncHandlerWithResult);
				default:
					throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
			}
		}

		private class SampleHandler : ICommandHandler<int>
		{
			public void Handle(int command)
			{
			}
		}

		private class SampleAsyncHandler : IAsyncCommandHandler<int>
		{
			public async Task HandleAsync(int command, CancellationToken cancellationToken = new CancellationToken())
			{
				await new Task(() => { });
			}
		}

		private class SampleAsyncHandlerWithResult : IAsyncResultCommandHandler<int, int>
		{
			public async Task<Result<Unit, int>> HandleAsync(int command, CancellationToken cancellationToken)
			{
				return await new Task<Result<Unit, int>>(() => Result.Succeed<Unit, int>(Unit.Value));
			}
		}
	}
}
