using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Attributes
{
	/// <summary>
	/// Attribute for tagging command handlers that will opt out of being wrapped in a transaction scope.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class NoTransactionScopeAttribute : Attribute
	{
	}
}
