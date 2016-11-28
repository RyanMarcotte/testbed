using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.CQS.IoC.Constants
{
	internal static class AppSettingsNames
	{
		public const string CacheType = "IQ.CQS.CacheType";

		public const string IncludeExceptionLoggingInterceptor = "IQ.CQS.IncludeExceptionLoggingInterceptor";

		public const string IncludePerformanceMetricsLoggingInterceptor = "IQ.CQS.IncludePerformanceMetricsLoggingInterceptor";

		public const string IncludeQueryResultCachingInterceptor = "IQ.CQS.IncludeQueryResultCachingInterceptor";

		public const string IncludeTransactionScopeInterceptor = "IQ.CQS.IncludeTransactionScopeInterceptor";
	}
}
