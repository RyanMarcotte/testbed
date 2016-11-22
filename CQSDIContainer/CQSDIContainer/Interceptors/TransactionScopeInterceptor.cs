using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Castle.Core;
using IQ.Platform.Framework.Common;

namespace CQSDIContainer.Interceptors
{
	public class TransactionScopeInterceptor : CQSInterceptorWithExceptionHandling
	{
		private static readonly object _consoleWriteLock = new object();

		private TransactionScope _scope;
		private bool _transactionCompletedSuccessfully;

		protected override void OnBeginInvocation(ComponentModel componentModel)
		{
			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] begin transaction scope for {componentModel.Implementation}");

			_scope = new TransactionScope();
		}

		protected override void OnReceiveReturnValueFromCommandHandlerInvocation(ComponentModel componentModel)
		{
			_transactionCompletedSuccessfully = true;
		}

		protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			_transactionCompletedSuccessfully = returnValue.IsSuccessfull;
		}

		protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(ComponentModel componentModel, Task returnValue)
		{
			_transactionCompletedSuccessfully = true;
		}

		protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			_transactionCompletedSuccessfully = returnValue.IsSuccessfull;
		}

		protected override void OnEndInvocation(ComponentModel componentModel)
		{
			if (_transactionCompletedSuccessfully)
			{
				lock (_consoleWriteLock)
					Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] transaction completed successfully for {componentModel.Implementation}");

				_scope.Complete();
			}
			
			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] ending transaction scope for {componentModel.Implementation}");

			_scope.Dispose();
		}

		protected override void OnException(ComponentModel componentModel, Exception ex)
		{
			_transactionCompletedSuccessfully = false;
		}
	}
}
