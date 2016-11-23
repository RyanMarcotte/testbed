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
		private static ConcurrentDictionary<InvocationInstance, TransactionScope> _transactionScopeForInvocationLookup = new ConcurrentDictionary<InvocationInstance, TransactionScope>();

		protected override void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] begin transaction scope for {componentModel.Implementation}");

			_transactionScopeForInvocationLookup.TryAdd(invocationInstance, new TransactionScope());
		}

		protected override void OnReceiveReturnValueFromQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(invocationInstance, componentModel, true);
		}

		protected override void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)
		{
			CompleteTransaction(invocationInstance, componentModel, true);
		}

		protected override void OnReceiveReturnValueFromCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			CompleteTransaction(invocationInstance, componentModel, true);
		}

		protected override void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(invocationInstance, componentModel, returnValue.IsSuccessfull);
		}

		protected override void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			CompleteTransaction(invocationInstance, componentModel, true);
		}

		protected override void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)
		{
			CompleteTransaction(invocationInstance, componentModel, returnValue.IsSuccessfull);
		}

		private void CompleteTransaction(InvocationInstance invocationInstance, ComponentModel componentModel, bool isSuccessful)
		{
			if (!isSuccessful)
				return;

			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] transaction completed successfully for {componentModel.Implementation}");

			TransactionScope scope;
			if (_transactionScopeForInvocationLookup.TryGetValue(invocationInstance, out scope))
				scope.Complete();
		}

		protected override void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)
		{
			lock (_consoleWriteLock)
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] ending transaction scope for {componentModel.Implementation}");

			TransactionScope scope;
			if (_transactionScopeForInvocationLookup.TryRemove(invocationInstance, out scope))
				scope.Dispose();
		}
	}
}
