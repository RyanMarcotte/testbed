using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Caching;
using CQSDIContainer.Interceptors.Caching.Interfaces;
using CQSDIContainer.Queries.Caching;
using CQSDIContainer.UnitTests.Interceptors._Arrangements;
using CQSDIContainer.UnitTests.Interceptors._Customizations;
using CQSDIContainer.UnitTests._Customizations;
using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using CQSDIContainer.UnitTests._TestUtilities;
using DoubleCache;
using DoubleCache.LocalCache;
using FakeItEasy;
using IQ.Platform.Framework.Common.CQS;
using Ploeh.AutoFixture;
using Xunit;

namespace CQSDIContainer.UnitTests.Interceptors
{
	/// <summary>
	/// Unit tests for the <see cref="CacheQueryResultInterceptor"/> class.
	/// </summary>
	public class CacheQueryResultInterceptorTests
	{
		[Theory]
		[SpecificExecutionResultForSpecificHandlerArrangement(true, CQSHandlerType.Query)]
		[SpecificExecutionResultForSpecificHandlerArrangement(true, CQSHandlerType.AsyncQuery)]
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

		#region Arrangements

		private class SpecificExecutionResultForSpecificHandlerArrangement : CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler
		{
			public SpecificExecutionResultForSpecificHandlerArrangement(bool invocationCompletesSuccessfully, CQSHandlerType handlerType)
				: base(typeof(LogAnyExceptionsInterceptorCustomization), invocationCompletesSuccessfully, handlerType, new CacheAsideCustomization(), new NullKernelCustomization(), new CacheItemFactoryInstanceRepositoryCustomization())
			{
			}

			protected override IEnumerable<object> AddAdditionalParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				var queryType = GetQueryType(handlerType);
				var resultType = typeof(int);

				// add queryType and resultType
				return new[] { queryType, resultType };
			}

			private static Type GetQueryType(CQSHandlerType handlerType)
			{
				// ReSharper disable once SwitchStatementMissingSomeCases
				switch (handlerType)
				{
					case CQSHandlerType.Query:
						return typeof(SampleQuery);

					case CQSHandlerType.AsyncQuery:
						return typeof(SampleAsyncQuery);

					default:
						throw new ArgumentOutOfRangeException(nameof(handlerType), handlerType, null);
				}
			}
		}

		#endregion

		#region Customizations

		private class LogAnyExceptionsInterceptorCustomization : CQSInterceptorWithExceptionHandlingCustomizationBase<CacheQueryResultInterceptor>
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
						var handlerInstanceType = c.GetArgument<Type>(0);
						var handlerInterface = handlerInstanceType.GetInterfaces().FirstOrDefault(x => x.IsGenericType);
						if (handlerInterface == null)
							throw new InvalidOperationException();

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

		#endregion
	}
}
