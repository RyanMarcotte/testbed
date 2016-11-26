using System;

namespace IQ.CQS.Attributes
{
	/// <summary>
	/// Attribute for tagging command handlers that will opt out of being wrapped in a transaction scope.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class NoTransactionScopeAttribute : Attribute
	{
	}
}
