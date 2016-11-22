# Parameter Objects and Handlers

## Parameter Objects
Parameter objects encapsulate a set of values (i.e. function parameters) that will be passed into your handlers.  Query parameter objects must all implement the `IQuery<TResult>` interface (from IQ.Platform.Framework.Common NuGet package), where `TResult` is the type of data you are returning from your query.  The interface itself contains no methods.  Command parameter objects are not required to implement any interface.

## Handlers
All handler types are provided by the IQ.Platform.Framework.Common NuGet package unless otherwise noted.  The names do a pretty good job of explaining what type of task they perform.  In general, **use synchronous handlers for performing operations on an RQ database** and **use asynchronous handlers for performing operations on a Platform service**.  There are currently other architectural considerations (outside the scope of this framework) that prevent us from doing everything asynchronously.
### `IQueryHandler<TQuery, TResult>`
Used to retrieve data from an RQ database.  Must implement `TResult Handle(TQuery query)` method.
### `IAsyncQueryHandler<TQuery, TResult>`
Used to retrieve data asychronously from Platform.  Must implement `Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken)` method.
### `ICommandHandler<TCommand>`
Used to modify data in an RQ database.  Must implement `void Handle(TCommand command)`.
### `IAsyncCommandHandler<TCommand>`
Used to modify data in Platform.  Must implement `Task HandleAsync(TCommand command, CancellationToken cancellationToken)` method.
### `IResultCommandHandler<TCommand, TError>`
Used to modify data in an RQ database.  Must implement `Result<Unit, TError> Handle(TCommand command)` method.  Unlike `ICommandHandler<TCommand>`, we return a `Result<Unit, TError>` object.  If the operation performed by your handler is successful, return `Result.Succeed<Unit, TError>(Unit.Value)`.  If the operation performed by your handler fails due to some expected error (for example, invalid command parameters), return `Result.Fail<Unit, TError>(TError error)`.
### `IAsyncResultCommandHandler<TCommand, TError>`
Used to modify data in Platform.  Must implement `Task<Result<Unit, TError>> HandleAsync(TCommand command, CancellationToken cancellationToken)` method.  The same result-object conventions used for `IResultCommandHandler<TCommand, TError>` apply here.
### `IDatabaseInsertionCommandHandler<TCommand, TErrorCode>` (RQ specific)
Used to insert a single record into an RQ database (for example, an invoice main-details record).  Must implement `Result<DatabaseInsertionSuccess, DatabaseInsertionError<TErrorCode>> Handle(TCommand command)` method.  The same result-object conventions used for `IResultCommandHandler<TCommand, TError>` and `IAsyncResultCommandHandler<TCommand, TError>` apply here, but with some more complex return types.

The `DatabaseInsertionSuccess` class wraps an integer and string identifier pair (for example, `InvoiceID` = 1337 and `InvoiceIDByStore` = "84WEAIN2000").

The `DatabaseInsertionError<TErrorCode>` class wraps a `TErrorCode` enum value (this is a type constraint) and an error message string.  In cases where you do not require special enum values for database insertion failures, you can use the `NullDatabaseInsertionErrorCode` enum type, which only defines the `ErrorOccurred` enum value.  Convenience methods exist for generating `DatabaseInsertionError<TErrorCode>` class instances and can be found in the `DatabaseInsertionError` static class.

### FAQ
#### The command handler interfaces return very specific things...  Why can't commands return anything I want?
Recall that, by convention, commands "change the state of a system but do not return a value".  This is why the `IResultCommandHandler<,>` and `IAsyncResultCommandHandler<,>` interfaces return `Unit.Value` on success.  `Unit.Value` is essentially the void return type.  We have made a slight exception to this rule for `IDatabaseInsertionCommandHandler<,>` as explained above.  These restrictions are in place to prevent pattern abuse.  A command that returns a value is a query with a side-effect, which is violates the principles of CQS.  That is something we are trying to avoid.  If what you are trying to accomplish with your handler is not supported by any of the above interfaces, pull a senior developer aside and talk through your design.

#### Should I be throwing exceptions within my handlers?
*TLDR: Definitely not, because the framework treats expected errors and exceptions differently.*

The question you should ask is:  Am I actually handling an exceptional case?  For example, losing a connection to the database is an exceptional case, and so we should throw an exception.  Failing to find some record corresponding to an ID you've specified is not an exceptional case.  You expect this to happen if the ID you supply to the handler is invalid.  The framework uses the concept of [option types](https://en.wikipedia.org/wiki/Option_type) for handling expected errors.

You should choose an appropriate handler that returns a `Result<TSuccess, TFailure>` object (i.e. an option type) to encapsulate these types of cases.  The `IResultCommandHandler<TCommand, TError>`, `IAsyncResultCommandHandler<TCommand, TError>`, and `IDatabaseInsertionCommandHandler<TCommand, TErrorCode>` interfaces all support this pattern.  The use of this option type ensures that consumers of your handler can identify all possible failure cases without actually looking at your handler implementation.

## Naming Conventions
Please adhere to the following conventions so that it is easy for other developers to understand what your query / commands are for:
- query parameter object class names should be descriptive and suffixed with "Query" (for example, `GetEmployeeIDFromSSOIDQuery` or `GetLocationIDFromLocationEntityIDQuery`); similarly, command parameter object class names should be descriptive and suffixed with "Command" (for example, `AssignSingleLocationToEmployeeUsingPlatformIDsCommand`)
- the class name of a handler should match the class name of its corresponding query / command parameter object (for example, the handler for `GetEmployeeIDFromSSOIDQuery` should be named `GetEmployeeIDFromSSOIDQueryHandler` and the handler for `AssignSingleLocationToEmployeeUsingPlatformIDsCommand` should be named `AssignSingleLocationToEmployeeUsingPlatformIDsCommandHandler`)
- we do not include a descriptor in the class name for what type of query / command a handler is (for example, `AssignSingleLocationToEmployeeUsingPlatformIDsCommandHandler` is an implementation of the `IResultCommandHandler<TCommand, TError>` interface); all the handler interfaces have different method signatures for `Handle` / `HandleAsync`, so you will know what type of handler it is when you go use it
- there should only be a one-to-one relationship between a parameter object and its handler (i.e. a parameter object is handled by exactly one handler and vice versa); this is enforced for queries due to the `IQuery<TResult>` type constraint on query parameter objects

### FAQ
#### Your class names are really long...  Can I shorten it?
The particular examples given above have long names, yes.  The primary concern is that your class name accurately describes what you are querying for / what action you are performing.  There are going to be many, many classes and it helps everyone if uses descriptive names so that developers don't have to look at your implementation to figure out what it does.

If your descriptive name includes more than eight words (disclaimer: totally arbitrary number), your handler may be doing too much.  In cases like that, you may want to split it up into smaller handlers that each perform some part of the total work that needs to be done.  The smaller handlers also allow them to possibly be reused.

#### What about class bloat?
With RQ being as large as it is, we have two options: use a smaller number of large classes, or use a larger number of small classes.  As many can atest, we've been choosing the first option for a while and it has made many parts of our codebase unyieldly and difficult to change.  Tight coupling, incredibly broad scope, or some combination thereof often makes working with these classes a bad developer experience (SaleInvoiceController, I'm looking at you).  Jumping around a 3K-line class file sucks.  By breaking things up into small classes with narrow scope, we can product code that is easier to understand and is unit-testable.

## Data Services / REST Clients
Obviously, we need to make calls to an RQ database or Platform service to retrieve or modify data.  How should we structure that?

For RQ database calls, you should create a new class inheriting from the `DataServiceSetBaseForCQS` class.  The new class should then contain a small set of related methods.  **Avoid jamming new methods into an existing data service**.  In fact, pulling methods out of an existing monolithic data service class and placing those methods in a new smaller data service class is encouraged.  Just be mindful that this can be a difficult task depending on how many places those data service methods are used.

TODO: add `RestClientForCQS` class to RQ (inherit from RestClientV2)

**Your handlers should not contain any bootstrapping code required to make service calls**.  In other words, your handlers should not be creating SqlCommand objects or issuing REST calls themselves.  This work should be done within your data service / REST client class.  Your handler then takes an interface for that data service as a dependency (i.e. your handler's constructor takes an instance of that interface as an argument).  Essentially, your handler contains your processing logic (which can be unit-tested) and your data service / REST client class is responsible for executing a SqlCommand or issuing the appropriate REST call.

### FAQ
#### Why the extra abstraction for calls into data services / REST clients?
The primary reason for this is so that data service / REST client calls can be mocked for unit tests on your handlers.  Have to make a call to the database to retrieve data?  You can return whatever data you want by creating a mock of the interface responsible for making that database / REST call.

#### I'm still not convinced...
The extra abstraction also makes it easy for us to swap out implementations.  For example, we could introduce a new REST client base class (i.e. RestClient vs RestClientV2).  With the extra abstraction, you just have to create a new class that:
- inherits from the new REST client class
- implements the interface your handler requires

In other words, **you do not have to modify any handlers to take advantage of the new REST client** (unless you are also modifying method contracts as part of your refactor)!!  Who's a fan of doing less work?

## Putting It All Together
Coming soon
