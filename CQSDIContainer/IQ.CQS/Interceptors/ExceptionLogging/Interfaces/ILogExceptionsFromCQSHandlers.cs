using System;

namespace IQ.CQS.Interceptors.ExceptionLogging.Interfaces
{
	/// <summary>
	/// Interface for an object responsible for logging exceptions.
	/// </summary>
	public interface ILogExceptionsFromCQSHandlers
	{
		/// <summary>
		/// Log an exception.
		/// </summary>
		/// <param name="ex">The exception.</param>
		void LogException(Exception ex);
	}
}