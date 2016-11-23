using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Transactions;
using CQSDIContainer.Interceptors.Exceptions;
using CQSDIContainer.Interceptors.TransactionScopes.Interfaces;

namespace CQSDIContainer.Interceptors.TransactionScopes
{
	public class TransactionScopeManagerForCQSHandlers : IManageTransactionScopesForCQSHandlers
	{
		private static readonly ConcurrentDictionary<InvocationInstance, TransactionScope> _transactionScopeForInvocationLookup = new ConcurrentDictionary<InvocationInstance, TransactionScope>();

		public int NumberOfOpenTransactionScopes => _transactionScopeForInvocationLookup.Count;

		public void OpenTransactionScopeForInvocationInstance(InvocationInstance invocationInstance)
		{
			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] begin transaction scope for {invocationInstance.ComponentModelImplementationType}");

			_transactionScopeForInvocationLookup.TryAdd(invocationInstance, new TransactionScope());
		}

		public void CompleteTransactionScopeForInvocationInstance(InvocationInstance invocationInstance)
		{
			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] transaction completed successfully for {invocationInstance.ComponentModelImplementationType}");

			TransactionScope scope;
			if (_transactionScopeForInvocationLookup.TryGetValue(invocationInstance, out scope))
				scope.Complete();
			else
				throw new TransactionScopeNotFoundForInvocationException(invocationInstance);
		}

		public void DisposeTransactionScopeForInvocationInstance(InvocationInstance invocationInstance)
		{
			Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] ending transaction scope for {invocationInstance.ComponentModelImplementationType}");

			TransactionScope scope;
			if (_transactionScopeForInvocationLookup.TryRemove(invocationInstance, out scope))
				scope.Dispose();
			else
				throw new TransactionScopeNotFoundForInvocationException(invocationInstance);
		}
	}
}
