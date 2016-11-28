# Interceptors and Contributors

## Cross-Cutting Concerns
If you take a step through some areas of the RQ codebase - especially the old areas - you'll notice that there is a lot of duplicated code.  In cases where we decide to modify the behavior of this code, we have to go investigate all the areas where the behavior is used and make the appropriate modifications.  If we're smart about it, we'll remove duplication while we're at it.

One area where duplication occurs is in our application of *cross-cutting concerns*.  [This post on Stackflow](http://stackoverflow.com/a/25779679) does a great job of illustrating the concept (literally).
>A concern is a term that refers to a part of the system divided on the basis of the functionality.

>A cross-cutting concern is a concern which is applicable throughout the application and it affects the entire application.

Examples of cross-cutting concerns are logging and performing security checks.  These functions are performed regardless of the application area you are working in.  Wouldn't it be great to automatically perform these tasks instead of having to rewrite them over and over again?  We can!  Enter interceptors.

## Interceptors

Anyone looking for information about the underlying technology can consult the [Castle Windsor documentation](https://github.com/castleproject/Windsor/blob/master/docs/interceptors.md).  This guide's scope is limited to how they are applied within the RQ.Server.CQS framework.

The basic idea is that an interceptor *intercepts a invocation and can perform some work around call*.  An invocation is just a function call.  In our case, these invocations are `Handle` and `HandleAsync` methods on our handler implementations.

All interceptors within the framework inherit from the [`CQSInterceptor`](../CQSDIContainer/Interceptors/_CQSInterceptor.cs) abstract base class.  Another abstract base class, [`CQSInterceptorWithExceptionHandling`](../CQSDIContainer/Interceptors/_CQSInterceptorWithExceptionHandling.cs) (which inherits from `CQSInterceptor`), greatly simplifies the interception logic for derived classes.  The internals of how interceptions are performed by these classes is complex (need to handle sync and async methods differently, reflection is needed in some cases, etc.), so we will only consider derivations of the `CQSInterceptorWithExceptionHandling` class here.

### Code Flow for CQSInterceptorWithExceptionHandling
The interception logic flows as such, assuming no exception is thrown:
- `OnBeginInvocation`
- *perform invocation*
- `OnReceiveReturnValueFromInvocation`
- `OnEndInvocation`

If an exception is thrown, then interception is aborted and `OnException` is called before throwing the exception back to the caller.

We use a couple interfaces and classes from Castle.Windsor in order to provide enough information to developers for distinguishing between different invocations.  If you're interested, you can look at the source code by clicking the links below.
- [`IInvocation`](https://github.com/castleproject/Core/blob/master/src/Castle.Core/DynamicProxy/IInvocation.cs) is an abstraction of our function call
- [`ComponentModel`](https://github.com/castleproject/Windsor/blob/master/src/Castle.Windsor/Core/ComponentModel.cs) is an abstraction of the object making the function call (i.e. the class type)

We also introduce an [`InvocationInstance`](../CQSDIContainer/Interceptors/_InvocationInfo.cs) class for encapsulating a unique invocation.  That is, each call to `Handle` / `HandleAsync` for a given CQS handler class will generate a different `InvocationInstance` object.

All of the following methods are declared `virtual`, so they can be overwritten.  The methods do nothing unless a developer specifically overrides them in their derived class.

#### `OnBeginInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)`
Called just before beginning handler invocation.  Use for setup.
#### `OnReceiveReturnValueFromInvocation`
Why are there no parameter arguments listed here?  This method name actually doesn't exist.  However, there are several variants of this method concept, one for each type of handler.  The `CQSInterceptorWithExceptionHandling` base class chooses the appropriate method to call based on the invoked handler's type.
##### `OnReceiveReturnValueFromQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)`
Called immediately after successfully returning from the invocation of a synchronous query handler invocation.  You have access to the return value if required, though it is not strongly typed.
##### `OnReceiveReturnValueFromAsyncQueryHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel, object returnValue)`
Called immediately after successfully returning from the invocation of an asynchronous query handler invocation.  Similar to the synchronous variant, you have access to the underlying `Task`'s return value (not the `Task` object itself).
##### `OnReceiveReturnValueFromCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)`
Called immediately after successfully returning from the invocation of a synchronous command handler invocation that does not return any value.
##### `OnReceiveReturnValueFromResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)`
Called immediately after successfully returning from the invocation of an synchronous command handler that returns a result.  The method is generic, but the class is not, so you will not be able to access properties on your success / failure objects without some serious type-checking hackery.  Basically, you're only really interested in checking the `IsSuccessfull` flag on the return value.
##### `OnReceiveReturnValueFromAsyncCommandHandlerInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)`
Called immediately after successfully returning from the invocation of an asynchronous command handler.
##### `OnReceiveReturnValueFromAsyncResultCommandHandlerInvocation<TSuccess, TFailure>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<TSuccess, TFailure> returnValue)`
Called immediately after successfully returning from the invocation of an asynchronous command handler that returns a result.  All the extra considerations given above for `OnReceiveReturnValueFromResultCommandHandlerInvocation` apply here: just check the `IsSuccessfull` flag on the return value.
##### `OnReceiveReturnValueFromDatabaseInsertionCommandHandlerInvocation<TErrorCode>(InvocationInstance invocationInstance, ComponentModel componentModel, Result<DatabaseInsertionSuccess, DatabaseInsertionError<TErrorCode>> returnValue)`
Called immediately after successfully returning from the invocation of a database insertion command handler.  Again, because this is a generic method within a non-generic class, you just want to check the `IsSuccessfull` flag on the return value.
#### `OnEndInvocation(InvocationInstance invocationInstance, ComponentModel componentModel)`
Always called just before returning control to the caller (including when exceptions are thrown - this method is called as part of a `finally` block).  Use for teardown.
#### `OnException(InvocationInstance invocationInstance, ComponentModel componentModel, Exception ex)`
Called when an exception has been thrown by the interceptor or during invocation.

You can find the collection of interceptors in RQ.Server.CQS [here](../CQSDIContainer/Interceptors).  Note that aside from [the query-result caching interceptor](../CQSDIContainer/Interceptors/CacheQueryResultInterceptor.cs) (which requires special interception logic), the implementations of an interceptor are incredibly straight-forward.

## Contributors

Castle.Windsor uses the concept of *contributors* to customize how interceptors are applied to invocations at registration-time (i.e. on application start-up).  Anyone wanting to do a deeper dive can consult the [Castle Windsor documentation](https://github.com/castleproject/Windsor/blob/master/docs/componentmodel-construction-contributors.md).  Again, we limit the scope of our discussion to how contributors are used in the RQ.Server.CQS framework.

We have created an abstraction to make working with Castle.Windsor contributors easy: [the `CQSInterceptorContributor<TInterceptorType>` abstract class](../CQSDIContainer/Contributors/_CQSInterceptorContributor.cs).  A type constraint exists on `TInterceptorType`: it must be a `CQSInterceptor`.

### Basic Functionality
Classes that inherit from `CQSInterceptorContributor` only need to provide the implementations for the `HandlerTypesToApplyTo` property and the `ShouldApplyInterceptor(IKernel kernel, ComponentModel model)` method.

`HandlerTypesToApplyTo` can be any of the following enum values: `None`, `QueryHandlersOnly`, `CommandHandlersOnly`, and `AllHandlers`.  The names should be self-explanatory.

The `ShouldApplyInterceptor(IKernel kernel, ComponentModel model)` method returns a boolean value and is used to offer opt-in or opt-out rules for interceptors.  We use attributes or the existence of components within a Castle.Windsor kernel to determine if an interceptor is applied to a specific handler.  Some examples:
- the [`ExecutionTimeLoggingInterceptorContributor`](../CQSDIContainer/Contributors/ExecutionTimeLoggingContributor.cs) will only apply a `LogExecutionTimeInterceptor` to handlers tagged with the `LogExecutionTimeAttribute` attribute 
- the [`TransactionScopeContributor`](../CQSDIContainer/Contributors/TransactionScopeContributor.cs) will always apply a `TransactionScopeInterceptor` to command handlers unless the handler implementation is tagged with [`NoTransactionScopeAttribute`]
- the [`QueryResultCachingContributor`](../CQSDIContainer/Contributors/QueryResultCachingContributor.cs) will only apply a `QueryResultCachingInterceptor` to queries that have an accompanying [`IQueryCacheItemFactory<TQuery, TResult>`](../CQSDIContainer/Queries/Caching/IQueryCacheItemFactory.cs) implementation defined

### Dealing with Nested Handlers
We require special logic for *nested handlers*, which the `CQSInterceptorContributor` class handles for us.  A handler is *nested* if it has been injected into another handler as a dependency; that is, it is a handler contained within another handler (remember when we were talking about being able to compose complex handlers from simpler ones?).

For example, consider a command handler `H` that updates records in the RQ database.  `H` contains references to three simpler, nested command handlers: `a`, `b`, and `c`.  The three nested handlers each perform some smaller amount of work to complete the RQ database update operation.  We only require a single transaction scope around `H`.  Since `a`, `b`, and `c` are inside `H`, they will also be executed within the one transaction scope.  If any of `a`, `b`, or `c` fail, then the entire operation performed by `H` up to that point is rolled back.  Having transaction scopes on each of `a`, `b`, and `c` is redundant in most RQ cases.

By default, interceptors only apply to the outermost handler (`H` in our example above).  If an interceptor should be also be used for any nested handlers, tag the interceptor class with [`ApplyToNestedHandlersAttribute`](../CQSDIContainer/Interceptors/Attributes/ApplyToNestedHandlersAttribute.cs).  See [the query-result caching interceptor](../CQSDIContainer/Interceptors/CacheQueryResultInterceptor.cs) for an example.
