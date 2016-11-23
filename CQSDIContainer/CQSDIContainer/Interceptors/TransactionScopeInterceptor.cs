using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Castle.Core;
using Castle.DynamicProxy;
using IQ.Platform.Framework.Common;

namespace CQSDIContainer.Interceptors
{
	public class TransactionScopeInterceptor : CQSInterceptorWithExceptionHandling
	{
		private static readonly object _consoleWriteLock = new object();
		private static ConcurrentDictionary<IInvocation, TransactionScope> _transactionScopeForInvocationLookup = new ConcurrentDictionary<IInvocation, TransactionScope>();

		private TransactionScope _scope;
		
		protected override void OnBeginInvocation(ComponentModel componentModel)
		{
			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] begin transaction scope for {componentModel.Implementation}");

			_scope = new TransactionScope();
		}

		protected override void OnReceiveReturnValueFromQueryHandlerInvocation(ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(componentModel, true);
		}

		protected override void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(componentModel, true);
		}

		protected override void OnReceiveReturnValueFromCommandHandlerInvocation(ComponentModel componentModel)
		{
			CompleteTransaction(componentModel, true);
		}

		protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(componentModel, returnValue.IsSuccessfull);
		}

		protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(ComponentModel componentModel)
		{
			CompleteTransaction(componentModel, true);
		}

		protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(componentModel, returnValue.IsSuccessfull);
		}

		private void CompleteTransaction(ComponentModel componentModel, bool isSuccessful)
		{
			if (!isSuccessful)
				return;

			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] transaction completed successfully for {componentModel.Implementation}");

			_scope.Complete();
		}

		protected override void OnEndInvocation(ComponentModel componentModel)
		{
			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] ending transaction scope for {componentModel.Implementation}");

			_scope.Dispose();
		}
	}
}
