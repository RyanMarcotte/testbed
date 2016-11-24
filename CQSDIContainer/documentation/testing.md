# RQ.Server.CQS.UnitTests
The RQ.Server.CQS framework is fully covered by a suite of unit tests.  Let's keep it that way!  Here's some stuff that you should know prior to writing your own unit tests against components in the system.  Effort has been put in to simplify the process.

## Writing Unit Tests for Interceptors
Interceptors are by far the most difficult to unit test.  They must be able to handle each of the different handler types, whether or not invocations of those handlers succeed or fail, whether or not an exception is thrown...  The number of test cases balloons pretty quickly depending on the complexity of your handler.

To help ease the development of unit tests for interceptors, a framework has been built around it to handle a lot of the boilerplate required for creating common arrangements and customizations.

### Arrangements
[`CQSInterceptorWithExceptionHandlingArrangementBase`](../CQSDIContainer.UnitTests/_Arrangements/CQSInterceptorWithExceptionHandlingArrangementBase.cs)
[`CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase`](../CQSDIContainer.UnitTests/_Arrangements/CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase.cs)

### Customizations
[`CQSInterceptorWithExceptionHandlingCustomizationBase<TransactionScopeInterceptor>`](../CQSDIContainer.UnitTests/_Customizations/Utilities/CQSInterceptorWithExceptionHandlingCustomizationBase.cs)

## Writing Unit Tests for Contributors
Why?  The base class handles all the logic for you!  Derived classes should just be specifying parameters for the base class to act on, so you shouldn't have to write any additional tests *unless you're doing something complex / outside the prescribed pattern*.  Lucky you!
