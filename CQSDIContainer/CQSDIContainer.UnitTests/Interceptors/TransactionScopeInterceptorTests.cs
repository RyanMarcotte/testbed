using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.TransactionScopes.Interfaces;
using CQSDIContainer.UnitTests.Arrangements;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Customizations.Utilities;
using CQSDIContainer.UnitTests.TestUtilities;
using FakeItEasy;
using Ploeh.AutoFixture;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="TransactionScopeInterceptor"/> class.
	/// </summary>
	public class TransactionScopeInterceptorTests
	{
		[Theory]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.Query)]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.AsyncQuery)]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.Command)]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.AsyncCommand)]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.ResultCommand_Succeeds)]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.AsyncResultCommand_Succeeds)]
		public void OpensThenCompletesThenDisposesATransactionScopeForSuccessfulInvocation(TransactionScopeInterceptor sut, IInvocation invocation, ComponentModel componentModel)
		{
			sut.SetInterceptedComponentModel(componentModel);
			sut.Intercept(invocation);

			A.CallTo(() => sut.TransactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once)
				.Then(A.CallTo(() => sut.TransactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once))
				.Then(A.CallTo(() => sut.TransactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once));
		}

		[Theory]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.ResultCommand_Fails)]
		[InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType.AsyncResultCommand_Fails)]
		[InterceptedMethodThrowsAnExceptionArrangement]
		public void OpensThenDisposesATransactionScopeForInvocationsThatThrowExceptions(TransactionScopeInterceptor sut, IInvocation invocation, ComponentModel componentModel)
		{
			try
			{
				sut.SetInterceptedComponentModel(componentModel);
				sut.Intercept(invocation);
			}
			catch (InvocationFailedException)
			{
				// eat the exception since we expect it	
			}

			A.CallTo(() => sut.TransactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once)
				.Then(A.CallTo(() => sut.TransactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustHaveHappened(Repeated.Exactly.Once));

			A.CallTo(() => sut.TransactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).MustNotHaveHappened();
		}

		#region Arrangements

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class InterceptedMethodCompletesSuccessfullyArrangement : CQSInterceptorWithExceptionHandlingArrangementBase
		{
			public InterceptedMethodCompletesSuccessfullyArrangement(CQSHandlerType methodType)
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), true, methodType)
			{

			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		private class InterceptedMethodThrowsAnExceptionArrangement : CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase
		{
			public InterceptedMethodThrowsAnExceptionArrangement()
				: base(typeof(CQSInterceptorWithExceptionHandlingCustomization), false)
			{
				
			}
		}

		#endregion

		#region Customizations

		private class CQSInterceptorWithExceptionHandlingCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<TransactionScopeInterceptor>
		{
			public override TransactionScopeInterceptor CreateInterceptor(IFixture fixture, bool isAlreadyInitialized)
			{
				if (!isAlreadyInitialized)
				{
					fixture.Register(() =>
					{
						var transactionScopeManager = A.Fake<IManageTransactionScopesForCQSHandlers>();
						A.CallTo(() => transactionScopeManager.OpenTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
						A.CallTo(() => transactionScopeManager.CompleteTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
						A.CallTo(() => transactionScopeManager.DisposeTransactionScopeForInvocationInstance(A<InvocationInstance>._)).DoesNothing();
						return transactionScopeManager;
					});
				}

				return new TransactionScopeInterceptor(fixture.Create<IManageTransactionScopesForCQSHandlers>());
			}
		}

		#endregion
	}
}
