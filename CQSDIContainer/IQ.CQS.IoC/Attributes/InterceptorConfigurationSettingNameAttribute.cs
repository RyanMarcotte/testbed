using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.CQS.IoC.Contributors;

namespace IQ.CQS.IoC.Attributes
{
	/// <summary>
	/// Attribute used to tag classes derived from <see cref="CQSInterceptorContributor{TInterceptorType}"/>.
	/// Enables the enabling / disabling of interceptors via application configuration.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class InterceptorConfigurationSettingNameAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorConfigurationSettingNameAttribute"/> class.
		/// </summary>
		/// <param name="settingName">The name of the enable/disable setting corresponding to the contributor.</param>
		public InterceptorConfigurationSettingNameAttribute(string settingName)
		{
			SettingName = settingName;
		}

		/// <summary>
		/// The name of the setting corresponding to the interceptor contributor to enable / disable.
		/// </summary>
		public string SettingName { get; }
	}
}
