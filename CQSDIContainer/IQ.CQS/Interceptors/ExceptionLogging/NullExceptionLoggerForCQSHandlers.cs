using System;
using IQ.CQS.Interceptors.ExceptionLogging.Interfaces;

namespace IQ.CQS.Interceptors.ExceptionLogging
{
	internal class NullExceptionLoggerForCQSHandlers : ILogExceptionsFromCQSHandlers
	{
		public void LogException(Exception ex)
		{
			Console.WriteLine("An exception occured!!");
			Console.WriteLine(ex);
		}
	}
}