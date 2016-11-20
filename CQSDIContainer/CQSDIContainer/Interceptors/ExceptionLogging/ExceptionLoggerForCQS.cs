using System;
using CQSDIContainer.Interceptors.ExceptionLogging.Interfaces;

namespace CQSDIContainer.Interceptors.ExceptionLogging
{
	public class ExceptionLoggerForCQS : ILogExceptionsFromCQSHandlers
	{
		public void LogException(Exception ex)
		{
			Console.WriteLine("An exception occured!!");
			Console.WriteLine(ex);
		}
	}
}