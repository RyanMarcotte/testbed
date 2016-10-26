using System;

namespace CQSDIContainer.Attributes
{
	/// <summary>
	/// Attribute for tagging command handlers and query handlers that will have their execution time printed to the console output window.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class LogExecutionTimeToConsoleAttribute : Attribute
	{
	}
}
