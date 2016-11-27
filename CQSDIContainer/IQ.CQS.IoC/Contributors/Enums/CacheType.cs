using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.CQS.IoC.Contributors.Enums
{
	/// <summary>
	/// Enumerates the different caching systems supported by IQ.CQS.
	/// </summary>
	public enum CacheType
	{
		/// <summary>
		/// Do not use caching.
		/// </summary>
		None,

		/// <summary>
		/// Use a local memory cache.
		/// </summary>
		Local
	}
}
