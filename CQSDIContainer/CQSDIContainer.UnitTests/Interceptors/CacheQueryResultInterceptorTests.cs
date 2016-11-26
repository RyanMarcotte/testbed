using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Caching.Interfaces;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;
using CQSDIContainer.UnitTests.Customizations;
using CQSDIContainer.UnitTests.Interceptors._Arrangements;
using CQSDIContainer.UnitTests.Interceptors._Customizations;
using CQSDIContainer.UnitTests._TestUtilities;
using FakeItEasy;
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

			// the result of the query should have been cached already, so do not run the intercepted method
			sut.Intercept(invocation);
			A.CallTo(() => invocation.Proceed()).MustHaveHappened(Repeated.Exactly.Once);
		}

		#region Arrangements

		private class SpecificExecutionResultForSpecificHandlerArrangement : CQSInterceptorArrangementBase_SpecificExecutionResultForSpecificHandler
		{
			public SpecificExecutionResultForSpecificHandlerArrangement(bool invocationCompletesSuccessfully, CQSHandlerType handlerType)
				: base(typeof(LogAnyExceptionsInterceptorCustomization), invocationCompletesSuccessfully, handlerType, new CacheAsideCustomization(), new KernelCustomization())
			{
			}

			protected override IEnumerable<object> AddAdditionalParametersBasedOnCQSHandlerType(IEnumerable<object> additionalParameters, CQSHandlerType handlerType)
			{
				// add queryType and resultType
				return base.AddAdditionalParametersBasedOnCQSHandlerType(additionalParameters, handlerType);
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
				return new CacheQueryResultInterceptor(fixture.Create<ICacheAside>, fixture.Create<IKernel>, fixture.Create<ILogCacheHitsAndMissesForQueryHandlers>());
			}
		}

		private class CacheAsideCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Register<ICacheAside>(() => new LocalCache());
			}
		}

		#endregion
	}
}
