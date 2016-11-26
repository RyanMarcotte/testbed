using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using DoubleCache;
using DoubleCache.LocalCache;
using FakeItEasy;
using FluentAssertions;
using IQ.CQS.Caching;
using IQ.CQS.Interceptors;
using IQ.CQS.Interceptors.Caching;
using IQ.CQS.Interceptors.Caching.Interfaces;
using IQ.CQS.UnitTests.Framework.Customizations;
using IQ.CQS.UnitTests.Framework.Enums;
using IQ.CQS.UnitTests.Framework.Exceptions;
using IQ.CQS.UnitTests.Framework.Utilities;
using IQ.CQS.UnitTests.Interceptors._Arrangements;
using IQ.CQS.UnitTests.Interceptors._Customizations;
using IQ.Platform.Framework.Common.CQS;
using Ploeh.AutoFixture;
using Xunit;

namespace IQ.CQS.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="CacheQueryResultInterceptor"/> class.
	/// </summary>
	public class CacheQueryResultInterceptorTests
	{
		[Theory]
		[QueryHandlerInvocationDoesNotThrowAnExceptionArrangement]
		public void CachesQueryResultIfNotInCacheAndDoesNotRunInterceptedMethodIfQueryResultAlreadyInCache(CacheQueryResultInterceptor sut, IInvocation invocation, Type queryType, Type resultType)
		{
			// the result of the query should not be in the cache, so proceed with the invocation
			A.CallTo(() => invocation.Proceed()).MustNotHaveHappened();
			sut.Intercept(invocation);
			A.CallTo(() => invocation.Proceed()).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => sut.CacheLogger.LogCacheMiss(queryType, resultType, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => sut.CacheLogger.LogCacheHit(queryType, resultType, A<string>._)).MustNotHaveHappened();

			// the result of the query should have been cached already, so do not run the intercepted method
			sut.Intercept(invocation);
			A.CallTo(() => invocation.Proceed()).MustHaveHappened(Repeated.Exactly.Once);
			A.CallTo(() => sut.CacheLogger.LogCacheMiss(queryType, resultType, A<string>._)).MustHaveHappened(Repeated.Exactly.Once); // no additional calls since last time
			A.CallTo(() => sut.CacheLogger.LogCacheHit(queryType, resultType, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Theory]
		[InvocationOfQueryHandlerThatReturnsValueTypeThrowsAnExceptionArrangement]
		public void DoesNotCacheResultIfInvocationOfQueryHandlerThatReturnsValueTypeThrowsAnException(CacheQueryResultInterceptor sut, IInvocation invocation, Type queryType, Type resultType)
		{
			Assert.Throws<InvocationFailedException>(() => sut.Intercept(invocation));
			A.CallTo(() => sut.CacheLogger.LogCacheMiss(queryType, resultType, A<string>._)).MustNotHaveHappened();
			A.CallTo(() => sut.CacheLogger.LogCacheHit(queryType, resultType, A<string>._)).MustNotHaveHappened();
		}

		[Theory]
		[InvocationOfQueryHandlerThatReturnsReferenceTypeThrowsAnExceptionArrangement]
		[AsyncQueryHandlerInvocationThrowsAnExceptionArrangement]
		public void DoesNotCacheResultIfInvocationOfQueryHandlerThrowsAnException(CacheQueryResultInterceptor sut, IInvocation invocation, Type queryType, Type resultType)
		{
			try
			{
				sut.Intercept(invocation);
			}
			catch (TargetInvocationException ex)
			{
				// we check to ensure that the inner exception is what we expect
				Assert.True(ex.InnerException != null && ex.InnerException.GetType() == typeof(InvocationFailedException));
			}
			
			A.CallTo(() => sut.CacheLogger.LogCacheMiss(queryType, resultType, A<string>._)).MustNotHaveHappened();
			A.CallTo(() => sut.CacheLogger.LogCacheHit(queryType, resultType, A<string>._)).MustNotHaveHappened();
		}

		#region Arrangements

		private abstract class SpecificExecutionResultForQueryHandlerArrangement : CQSInterceptorArrangementBase_CommonExecutionResultForAllHandlerInvocations
		{
			protected SpecificExecutionResultForQueryHandlerArrangement(CQSHandlerTypeSelector handlerTypeSelector, bool invocationCompletesSuccessfully)
				: base(typeof(CacheQueryResultInterceptorCustomization), handlerTypeSelector, invocationCompletesSuccessfully, new CacheAsideCustomization(), new NullKernelCustomization(), new CacheItemFactoryInstanceRepositoryCustomization())
			{
			}

			protected override IEnumerable<object> AddAdditionalUnitTestMethodParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				var queryType = SampleCQSHandlerImplementationFactory.GetArgumentsUsedForHandleAndHandleAsyncMethodsForHandlerType(handlerType)[0].GetType();
				var resultType = SampleCQSHandlerImplementationFactory.GetUnderlyingReturnValueTypeForHandlerType(handlerType);

				// add queryType and resultType
				return new[] { queryType, resultType };
			}
		}

		private class QueryHandlerInvocationDoesNotThrowAnExceptionArrangement : SpecificExecutionResultForQueryHandlerArrangement
		{
			public QueryHandlerInvocationDoesNotThrowAnExceptionArrangement()
				: base(CQSHandlerTypeSelector.AllQueryHandlers, true)
			{
			}
		}

		private class InvocationOfQueryHandlerThatReturnsValueTypeThrowsAnExceptionArrangement : SpecificExecutionResultForQueryHandlerArrangement
		{
			public InvocationOfQueryHandlerThatReturnsValueTypeThrowsAnExceptionArrangement()
				: base(CQSHandlerTypeSelector.Query_ReturnsValueType, false)
			{
			}
		}

		private class InvocationOfQueryHandlerThatReturnsReferenceTypeThrowsAnExceptionArrangement : SpecificExecutionResultForQueryHandlerArrangement
		{
			public InvocationOfQueryHandlerThatReturnsReferenceTypeThrowsAnExceptionArrangement()
				: base(CQSHandlerTypeSelector.Query_ReturnsReferenceType, false)
			{
			}
		}

		private class AsyncQueryHandlerInvocationThrowsAnExceptionArrangement : SpecificExecutionResultForQueryHandlerArrangement
		{
			public AsyncQueryHandlerInvocationThrowsAnExceptionArrangement()
				: base(CQSHandlerTypeSelector.AsyncQueryHandlersOnly, false)
			{
			}
		}

		#endregion

		#region Customizations

		private class CacheAsideCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register<ICacheAside>(() => new MemCache());
			}
		}

		private class CacheItemFactoryInstanceRepositoryCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var cacheItemFactoryInstanceRepository = A.Fake<ICacheItemFactoryInstanceRepository>();
					A.CallTo(() => cacheItemFactoryInstanceRepository.GetCacheItemFactoryInformationForType(A<Type>._, A<IKernel>._)).ReturnsLazily(c =>
					{
						// verify that the handler instance type implements either IQueryHandler<,> or IAsyncQueryHandler<,>
						var handlerInstanceType = c.GetArgument<Type>(0);
						var handlerInterface = handlerInstanceType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && (x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) || x.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,>)));
						if (handlerInterface == null)
							throw new InvalidOperationException();

						// use reflection to create a new cache item factory instance using the specified types
						var queryType = handlerInterface.GenericTypeArguments[0];
						var resultType = handlerInterface.GenericTypeArguments[1];
						var factoryCreator = _createFactoryInstanceMethodInfo.MakeGenericMethod(queryType, resultType);

						return new CacheItemFactoryInfo(queryType, resultType, factoryCreator.Invoke(null, new object[] { }));
					});
					return cacheItemFactoryInstanceRepository;
				});
			}

			private static readonly MethodInfo _createFactoryInstanceMethodInfo = typeof(CacheItemFactoryInstanceRepositoryCustomization).GetMethod(nameof(CreateFactoryInstance), BindingFlags.Static | BindingFlags.NonPublic);

			private static IQueryCacheItemFactory<TQuery, TResult> CreateFactoryInstance<TQuery, TResult>() where TQuery : IQuery<TResult>
			{
				var queryCacheItemFactoryInstance = A.Fake<IQueryCacheItemFactory<TQuery, TResult>>();
				A.CallTo(() => queryCacheItemFactoryInstance.BuildKeyForQuery(A<TQuery>._)).ReturnsLazily(c => c.GetArgument<TQuery>(0).ToString());
				A.CallTo(() => queryCacheItemFactoryInstance.TimeToLive).Returns(TimeSpan.FromMinutes(5));
				return queryCacheItemFactoryInstance;
			}
		}

		private class CacheQueryResultInterceptorCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<CacheQueryResultInterceptor>
		{
			protected override void RegisterDependencies(IFixture fixture)
			{
				fixture.Register(() =>
				{
					var cacheLogger = A.Fake<ILogCacheHitsAndMissesForQueryHandlers>();
					A.CallTo(() => cacheLogger.LogCacheHit(A<Type>._, A<Type>._, A<string>._)).DoesNothing();
					A.CallTo(() => cacheLogger.LogCacheMiss(A<Type>._, A<Type>._, A<string>._)).DoesNothing();

					return cacheLogger;
				});
			}

			protected override CacheQueryResultInterceptor CreateInterceptor(IFixture fixture)
			{
				return new CacheQueryResultInterceptor(fixture.Create<ICacheAside>(), fixture.Create<IKernel>(), fixture.Create<ICacheItemFactoryInstanceRepository>(), fixture.Create<ILogCacheHitsAndMissesForQueryHandlers>());
			}
		}

		#endregion
	}
}
