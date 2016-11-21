using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Castle.Core;

namespace CQSDIContainer.Interceptors
{
	public class TransactionScopeInterceptor : CQSInterceptorWithExceptionHandling
	{
		private TransactionScope _scope;
		private bool _transactionCompletedSuccessfully = true;

		protected override void OnBeginInvocation(ComponentModel componentModel)
		{
			Console.WriteLine($"begin transaction scope for {componentModel.Implementation}");
			_scope = new TransactionScope();
		}

		protected override void OnEndInvocation(ComponentModel componentModel)
		{
			if (_transactionCompletedSuccessfully)
			{
				Console.WriteLine($"transaction completed successfully for {componentModel.Implementation}");
				_scope.Complete();
			}

			Console.WriteLine($"ending transaction scope for {componentModel.Implementation}");
			_scope.Dispose();
		}

		protected override void OnException(ComponentModel componentModel, Exception ex)
		{
			_transactionCompletedSuccessfully = false;
		}
	}
}
