using System;

namespace CQSDIContainer.Attributes
{
	/// <summary>
	/// Attribute for tagging command handlers that will be executed asynchronously.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class HandleCommandAsynchronouslyAttribute : Attribute
	{
		
	}
}
