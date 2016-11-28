# RQ.Server.CQS.UnitTests
The RQ.Server.CQS framework is fully covered by a suite of unit tests.  Let's keep it that way!  Here's some stuff that you should know prior to writing your own unit tests against components in the system.  Effort has been put in to simplify the process.

## Writing Unit Tests for Interceptors
Interceptors are by far the most difficult to unit test.  An interceptor must be able to *at least* handle each of the different handler types, whether or not invocations of those handlers succeed or fail, and whether or not an exception is thrown.  The number of test cases can balloon quickly depending on the complexity of your handler.

A framework has been built to help ease the development of unit tests for interceptors.  The framework is used to create arrangements and customizations that will be common to most interceptors.

### Arrangements
An [arrangement](http://wiki.c2.com/?ArrangeActAssert) creates the function parameters for your unit tests.  Object creation is done through the use of [customizations](http://blog.ploeh.dk/2011/03/18/EncapsulatingAutoFixtureCustomizations/).  There are two arrangement base classes available.

[`CQSInterceptorWithExceptionHandlingArrangementBase`](../CQSDIContainer.UnitTests/_Arrangements/CQSInterceptorWithExceptionHandlingArrangementBase.cs)

[`CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase`](../CQSDIContainer.UnitTests/_Arrangements/CQSInterceptorWithExceptionHandlingAllConfigurationsArrangementBase.cs)

### Customizations
[`CQSInterceptorWithExceptionHandlingCustomizationBase<TransactionScopeInterceptor>`](../CQSDIContainer.UnitTests/_Customizations/Utilities/CQSInterceptorWithExceptionHandlingCustomizationBase.cs)

## Writing Unit Tests for Contributors
Why?  The base class handles all the logic for you!  Derived classes should just be specifying parameters for the base class to act on, so you should not have to write any additional tests **unless you're doing something complex and/or not supported by the existing contributor pattern**.  Lucky you!
