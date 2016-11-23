using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Exceptions;
using CQSDIContainer.Infrastructure;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Interceptors
{
	/// <summary>
	/// Base class for interceptors applied to CQS handlers that work around exception handling logic.
	/// </summary>
	public abstract class CQSInterceptorWithExceptionHandling : CQSInterceptor
	{
		/// <summary>
		/// Called just before beginning handler invocation.  Use for setup.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnBeginInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous query handler invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromQueryHandlerInvocation(ComponentModel componentModel, object returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous query handler invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncQueryHandlerInvocation(ComponentModel componentModel, object returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of a synchronous command handler invocation that does not return any value.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnReceiveReturnValueFromCommandHandlerInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an synchronous command handler that returns a result.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncCommandHandlerInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called immediately after successfully returning from the invocation of an asynchronous command handler that returns a result.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="returnValue">The value returned from the invocation.</param>
		protected virtual void OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(ComponentModel componentModel, Result<TSuccess, TFailure> returnValue) { }

		/// <summary>
		/// Always called just before returning control to the caller.  Use for teardown.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		protected virtual void OnEndInvocation(ComponentModel componentModel) { }

		/// <summary>
		/// Called when an exception has been thrown during invocation.
		/// </summary>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		/// <param name="ex">The exception.</param>
		protected virtual void OnException(ComponentModel componentModel, Exception ex) { }

		#region Sealed Implementations

		protected sealed override void InterceptSync(IInvocation invocation, ComponentModel componentModel)
		{
			try
			{
				OnBeginInvocation(componentModel);
				invocation.Proceed();
				OnReceiveReturnValueFromSynchronousMethodInvocation(invocation, componentModel);
			}
			catch (Exception ex)
			{
				OnException(componentModel, ex);
				throw;
			}
			finally
			{
				OnEndInvocation(componentModel);
			}
		}

		protected sealed override void InterceptAsync(IInvocation invocation, ComponentModel componentModel, AsynchronousMethodType methodType)
		{
			try
			{
				OnBeginInvocation(componentModel);
				invocation.Proceed();
				switch (methodType)
				{
					case AsynchronousMethodType.Action:
						HandleAsync((Task)invocation.ReturnValue).GetAwaiter().GetResult();
						break;

					case AsynchronousMethodType.Function:
						ExecuteHandleAsyncWithResultUsingReflection(invocation);
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Invalid value!!");
				}
				OnReceiveReturnValueFromAsynchronousMethodInvocation(invocation, componentModel);
			}
			catch (Exception ex)
			{
				OnException(componentModel, ex);
				throw;
			}
			finally
			{
				OnEndInvocation(componentModel);
			}
		}

		/// <summary>
		/// Called immediately after successfully completing the synchronous invocation.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		private void OnReceiveReturnValueFromSynchronousMethodInvocation(IInvocation invocation, ComponentModel componentModel)
		{
			var invocationTypes = GetComponentModelInvocationTypes(componentModel);
			if (invocationTypes == InvocationTypes.None)
				throw new UnrecognizedCQSHandlerTypeException(componentModel);
			if (invocationTypes.IsMoreThanOneInvocationType())
				throw new HandlerClassImplementsMultipleHandlerInterfacesException(componentModel.Implementation, invocationTypes);

			if (invocationTypes.HasFlag(InvocationTypes.Query))
				OnReceiveReturnValueFromQueryHandlerInvocation(componentModel, invocation.ReturnValue);
			else if (invocationTypes.HasFlag(InvocationTypes.Command))
				OnReceiveReturnValueFromCommandHandlerInvocation(componentModel);
			else if (invocationTypes.HasFlag(InvocationTypes.ResultCommand))
				ExecuteOnReceiveReturnValueFromResultCommandHandlerInvocationUsingReflection(invocation, componentModel);
			else
				throw new UnexpectedHandlerTypeException(invocationTypes, InvocationTypes.Query | InvocationTypes.Command | InvocationTypes.ResultCommand);
		}

		#region OnReceiveReturnValueFromSynchronousMethodInvocation helpers

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> _onReceiveReturnValueFromSynchronousFunctionInvocationMethodLookup = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
		private static readonly MethodInfo _onReceiveReturnValueFromSynchronousFunctionInvocationMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(OnReceiveReturnValueFromResultCommandHandlerInvocation), BindingFlags.Instance | BindingFlags.NonPublic);

		private void ExecuteOnReceiveReturnValueFromResultCommandHandlerInvocationUsingReflection(IInvocation invocation, ComponentModel componentModel)
		{
			var successResultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var failureResultType = invocation.Method.ReturnType.GetGenericArguments()[1];
			var methodInfo = _onReceiveReturnValueFromSynchronousFunctionInvocationMethodLookup.GetOrAdd(Tuple.Create(successResultType, failureResultType), t => _onReceiveReturnValueFromSynchronousFunctionInvocationMethodInfo.MakeGenericMethod(t.Item1, t.Item2));
			invocation.ReturnValue = methodInfo.Invoke(this, new[] { componentModel, invocation.ReturnValue });
		}

		#endregion

		/// <summary>
		/// Called immediately after successfully completing the asynchronous invocation.
		/// </summary>
		/// <param name="invocation">The intercepted invocation.</param>
		/// <param name="componentModel">The component model associated with the intercepted invocation.</param>
		private void OnReceiveReturnValueFromAsynchronousMethodInvocation(IInvocation invocation, ComponentModel componentModel)
		{
			var invocationTypes = GetComponentModelInvocationTypes(componentModel);
			if (invocationTypes == InvocationTypes.None)
				throw new UnrecognizedCQSHandlerTypeException(componentModel);
			if (invocationTypes.IsMoreThanOneInvocationType())
				throw new HandlerClassImplementsMultipleHandlerInterfacesException(componentModel.Implementation, invocationTypes);

			if (invocationTypes.HasFlag(InvocationTypes.AsyncQuery))
				OnReceiveReturnValueFromAsyncQueryHandlerInvocation(componentModel, ((dynamic)invocation.ReturnValue).Result);
			else if (invocationTypes.HasFlag(InvocationTypes.AsyncCommand))
				OnReceiveReturnValueFromAsyncCommandHandlerInvocation(componentModel);
			else if (invocationTypes.HasFlag(InvocationTypes.AsyncResultCommand))
				ExecuteOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationUsingReflection(invocation, componentModel);
			else
				throw new UnexpectedHandlerTypeException(invocationTypes, InvocationTypes.AsyncQuery | InvocationTypes.AsyncCommand | InvocationTypes.AsyncResultCommand);
		}

		#region OnReceiveReturnValueFromAsynchronousMethodInvocation helpers

		private static readonly ConcurrentDictionary<Tuple<Type, Type>, MethodInfo> _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodLookup = new ConcurrentDictionary<Tuple<Type, Type>, MethodInfo>();
		private static readonly MethodInfo _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation), BindingFlags.Instance | BindingFlags.NonPublic);
		
		private void ExecuteOnReceiveReturnValueFromAsyncResultCommandHandlerInvocationUsingReflection(IInvocation invocation, ComponentModel componentModel)
		{
			var taskType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var successResultType = taskType.GetGenericArguments()[0];
			var failureResultType = taskType.GetGenericArguments()[1];
			var methodInfo = _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodLookup.GetOrAdd(Tuple.Create(successResultType, failureResultType), t => _onReceiveReturnValueFromAsynchronousFunctionInvocationMethodInfo.MakeGenericMethod(t.Item1, t.Item2));
			invocation.ReturnValue = methodInfo.Invoke(this, new object[] { componentModel, ((dynamic)invocation.ReturnValue).Result });
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

		#region Enums

		protected enum HandlerType
		{
			Synchronous
		}
		#endregion

		#region Helper Methods (for async)

		private static async Task HandleAsync(Task task)
		{
			await task;
		}

		private static async Task<T> HandleAsyncWithResult<T>(Task<T> task)
		{
			return await task;
		}

		private static readonly ConcurrentDictionary<Type, MethodInfo> _genericHandleAsyncWithResultMethodLookup = new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly MethodInfo _handleAsyncWithResultMethodInfo = typeof(CQSInterceptorWithExceptionHandling).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

		private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation)
		{
			var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
			var methodInfo = _genericHandleAsyncWithResultMethodLookup.GetOrAdd(resultType, t => _handleAsyncWithResultMethodInfo.MakeGenericMethod(t));
			invocation.ReturnValue = methodInfo.Invoke(this, new[] { invocation.ReturnValue });
		}

		#endregion

		#endregion
	}
}
