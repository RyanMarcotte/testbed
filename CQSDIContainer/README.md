# RQ.Server.CQS Framework

## What is CQS?
CQS stands for "command-query separation".  Rather than duplicating effort for explaining the fundamentals here, please read the following articles:
- http://martinfowler.com/bliki/CommandQuerySeparation.html (theory)
- https://en.wikipedia.org/wiki/Command%E2%80%93query_separation (more theory)
- https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91 (recommended! more theory for commands; includes code examples)
- https://cuttingedge.it/blogs/steven/pivot/entry.php?id=92 (recommended! more theory for queries; includes code examples)

In short, CQS is an architecture pattern that helps promote good design.  Rather than creating a single method in a data service class that does everything we need it to (as we have been doing in the RQ.Server project), we create a set of small, discrete classes that each handle one specific task.  From Martin Fowler's page linked above:
>The fundamental idea is that we should divide an object's methods into two sharply separated categories:
>- Queries: Return a result and do not change the observable state of the system (are free of side effects).
>- Commands: Change the state of a system but do not return a value.

Through this design, we keep code modular and easy to understand.  We can also create more complex queries and commands by injecting a set of simpler queries and commands as dependencies.   **Note that queries should never execute commands, as the point of queries is to not modify the system state** ("asking a question should not change the answer").  Additionally, because we're breaking our code up into small pieces, we can more effectively mock objects and write unit tests.

## What is RQ.Server.CQS?
RQ.Server.CQS is a framework designed to facilitate the development of code adhering to the CQS architecture pattern.  The idea is that individual developers should only concern themselves with writing up handlers and the data service / REST client calls those handlers use.  The framework does all the heavy lifting required for applying cross-cutting concerns (exception logging, performance metrics logging, caching query results).  This isn't too different from what we're doing already.  We're just structuring our code differently and following a different set of conventions.

We leverage CQS interfaces defined within the `IQ.Platform.Framework.Common NuGet` package.  The bulk of the magic makes use of the Castle.Windsor dependency injection framework for automagically hooking things up.

## System Information
You can find information about individual framework components in the documents linked below:
- [Getting Started](documentation/gettingStarted.md)
- [Parameter Objects and Handlers (How do I write code using CQS?)](documentation/parameterObjectsAndHandlers.md)
- [Interceptors and Contributors (How do I apply cross-cutting concerns to my queries and commands?)](documentation/interceptorsAndContributors.md)
- [RQ.Server.CQS.UnitTests (Anything I should be aware of when writing unit tests for the system?](documentation/testing.md)
