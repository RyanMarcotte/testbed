using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using IQ.CQS.Interceptors.Enums;
using IQ.CQS.Interceptors.Exceptions;
using IQ.Vanilla;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Interceptors
{
	/// <summary>
	/// Base class for interceptors applied to CQS handlers that work around exception handling logic.
	/// </summary>
	public abstract class CQSInterceptorWithExceptionHandling : CQSInterceptor
	{
		/// <summary>
		/// Called just before beginning handler invocation.  Use for setup.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous query handler invocation.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous query handler invocation.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous command handler invocation that does not return any value.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnReceiveReturnValueFromCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an synchronous command handler that returns a result.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler that returns a result.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue) { }

		/// <summary>
		/// Always called just before returning control to the caller.  Use for teardown.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel) { }

		/// <summary>
		/// Called when an exception has been thrown during invocation.
		/// </summary>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="ex">The exception.</param>
		protected virtual void OnException(InvocationInstance invocationInstance, ComponentModel componentModel, Exception ex) { }

		#region Sealed Implementations

		/// <summary>
		/// Interception logic for synchronous handlers.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected sealed override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			var invocationInstance = new InvocationInstance(invocation, componentModel);
			try
			{
				OnBeginInvocation(invocationInstance, componentModel);
				invocation.Proceed();
				OnReceiveReturnValueFromSynchronousMethodInvocation(invocation, componentModel, invocationInstance);
			}
			catch (Exception ex)
			{
				OnException(invocationInstance, componentModel, ex);
				throw;
			}
			finally
			{
				OnEndInvocation(invocationInstance, componentModel);
			}
		}

		/// <summary>
		/// Interception logic for asynchronous handlers.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="methodType">The asynchronous method type.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected sealed override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
		{
			var invocationInstance = new InvocationInstance(invocation, componentModel);
			try
			{
				OnBeginInvocation(invocationInstance, componentModel);
				invocation.Proceed();
				((dynamic)invocation.ReturnValue).GetAwaiter().GetResult();
				OnReceiveReturnValueFromAsynchronousMethodInvocation(invocation, componentModel, invocationInstance);
			}
			catch (Exception ex)
			{
				OnException(invocationInstance, componentModel, ex);
				throw;
			}
			finally
			{
				OnEndInvocation(invocationInstance, componentModel);
			}
		}

		/// <summary>
		/// Called immediately after successfully completing the synchronous invocation.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		private void OnReceiveReturnValueFromSynchronousMethodInvocation(IInvocation invocation, ComponentModel componentModel, InvocationInstance invocationInstance)
		{
			var invocationTypes = GetComponentModelInvocationTypes(componentModel);
			if (invocationTypes == InvocationTypes.None)
				throw new ComponentDoesNotImplementAnyRecognizedCQSHandlerInterfacesException(componentModel);
			if (invocationTypes.IsMoreThanOneInvocationType())
				throw new CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException(componentModel, invocationTypes);

			if (invocationTypes.HasFlag(InvocationTypes.Query))
				OnReceiveReturnValueFromQueryHandlerInvocation(invocationInstance, componentModel, invocation.ReturnValue);
			else if (invocationTypes.HasFlag(InvocationTypes.Command))
				OnReceiveReturnValueFromCommandHandlerInvocation(invocationInstance, componentModel);
			else if (invocationTypes.HasFlag(InvocationTypes.ResultCommand))
				ExecuteOnReceiveReturnValueFromResultCommandHandlerInvocationUsingReflection(invocation, componentModel, invocationInstance);
			else
				throw new UnexpectedCQSHandlerTypeException(invocationTypes, InvocationTypes.Query | InvocationTypes.Command | InvocationTypes.ResultCommand);
		}

		#region OnReceiveReturnValueFromSynchronousMethodInvocation helpers

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> _onReceiveReturnValueFromSynchronousFunctionInvocationMethodLookup = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
		private static readonly MethodInfo _onReceiveReturnValueFromSynchronousFunctionInvocationMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(OnReceiveReturnValueFromResultCommandHandlerInvocation), BindingFlags.Instance | BindingFlags.NonPublic);

		private void ExecuteOnReceiveReturnValueFromResultCommandHandlerInvocationUsingReflection(IInvocation invocation, ComponentModel componentModel, InvocationInstance invocationInstance)
		{
			var successResultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var failureResultType = invocation.Method.ReturnType.GetGenericArguments()[1];
			var methodInfo = _onReceiveReturnValueFromSynchronousFunctionInvocationMethodLookup.GetOrAdd(Tuple.Create(successResultType, failureResultType), t => _onReceiveReturnValueFromSynchronousFunctionInvocationMethodInfo.MakeGenericMethod(t.Item1, t.Item2));
			invocation.ReturnValue = methodInfo.Invoke(this, new[] { invocationInstance, componentModel, invocation.ReturnValue });
		}

		#endregion

		/// <summary>
		/// Called immediately after successfully completing the asynchronous invocation.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="invocationInstance">The instance of the intercepted invocation.</param>
		private void OnReceiveReturnValueFromAsynchronousMethodInvocation(IInvocation invocation, ComponentModel componentModel, InvocationInstance invocationInstance)
		{
			var invocationTypes = GetComponentModelInvocationTypes(componentModel);
			if (invocationTypes == InvocationTypes.None)
				throw new ComponentDoesNotImplementAnyRecognizedCQSHandlerInterfacesException(componentModel);
			if (invocationTypes.IsMoreThanOneInvocationType())
				throw new CQSHandlerClassImplementsMultipleCQSHandlerInterfacesException(componentModel, invocationTypes);

			if (invocationTypes.HasFlag(InvocationTypes.AsyncQuery))
				OnReceiveReturnValueFromAsyncQueryHandlerInvocation(invocationInstance, componentModel, ((dynamic)invocation.ReturnValue).Result);
			else if (invocationTypes.HasFlag(InvocationTypes.AsyncCommand))
				OnReceiveReturnValueFromAsyncCommandHandlerInvocation(invocationInstance, componentModel);
			else if (invocationTypes.HasFlag(InvocationTypes.AsyncResultCommand))
				ExecuteOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationUsingReflection(invocation, componentModel, invocationInstance);
			else
				throw new UnexpectedCQSHandlerTypeException(invocationTypes, InvocationTypes.AsyncQuery | InvocationTypes.AsyncCommand | InvocationTypes.AsyncResultCommand);
		}

		#region OnReceiveReturnValueFromAsynchronousMethodInvocation helpers

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodLookup = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
		private static readonly MethodInfo _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation), BindingFlags.Instance | BindingFlags.NonPublic);
		
		private void ExecuteOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationUsingReflection(IInvocation invocation, ComponentModel componentModel, InvocationInstance invocationInstance)
		{
			var taskType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var successResultType = taskType.GetGenericArguments()[0];
			var failureResultType = taskType.GetGenericArguments()[1];
			var methodInfo = _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodLookup.GetOrAdd(Tuple.Create(successResultType, failureResultType), t => _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodInfo.MakeGenericMethod(t.Item1, t.Item2));
			methodInfo.Invoke(this, new object[] { invocationInstance, componentModel, ((dynamic)invocation.ReturnValue).Result });
		}

		#endregion

		#region OnReceiveReturnValueFromSynchronousMethodInvocation, OnReceiveReturnValueFromAsynchronousMethodInvocation helpers

		private static InvocationTypes GetComponentModelInvocationTypes(ComponentModel componentModel)
		{
			var invocationType = InvocationTypes.None;
			var genericInterfaceTypes = new HashSet<Type>(componentModel.Implementation.GetInterfaces().Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition()));
			if (genericInterfaceTypes.Contains(typeof(IQueryHandler<,>)))
				invocationType |= InvocationTypes.Query;
			if (genericInterfaceTypes.Contains(typeof(IAsyncQueryHandler<,>)))
				invocationType |= InvocationTypes.AsyncQuery;
			if (genericInterfaceTypes.Contains(typeof(ICommandHandler<>)))
				invocationType |= InvocationTypes.Command;
			if (genericInterfaceTypes.Contains(typeof(IAsyncCommandHandler<>)))
				invocationType |= InvocationTypes.AsyncCommand;
			if (genericInterfaceTypes.Contains(typeof(IResultCommandHandler<,>)))
				invocationType |= InvocationTypes.ResultCommand;
			if (genericInterfaceTypes.Contains(typeof(IAsyncResultCommandHandler<,>)))
				invocationType |= InvocationTypes.AsyncResultCommand;
			
			return invocationType;
		}

		#endregion

		#endregion
	}
}
