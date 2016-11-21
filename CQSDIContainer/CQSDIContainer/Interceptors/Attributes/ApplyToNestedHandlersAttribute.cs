using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Interceptors.Attributes
{
	/// <summary>
	/// Attribute for tagging interceptors that can be applied to nested handlers.
	/// </summary>
	/// <remarks>
	/// A handler is 'nested' if it is injected into another handler as a dependency.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public class ApplyToNestedHandlersAttribute : Attribute
	{
	}
}
