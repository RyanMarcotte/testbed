using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using CQSDIContainer.Interceptors.Session.Interfaces;

namespace CQSDIContainer.Interceptors
{
	public class DisableInterceptionForNestedHandlersInterceptor : CQSInterceptorWithExceptionHandling
	{
		protected override bool ApplyToNestedHandlers => true;

		protected override void OnBeginInvocation(ComponentModel componentModel)
		{
			((ICQSHandlerSessionWithAbilityToDisableInterceptors)InvocationSession).InterceptorsDisabled = true;
		}

		protected override void OnEndInvocation(ComponentModel componentModel)
		{
			((ICQSHandlerSessionWithAbilityToDisableInterceptors)InvocationSession).InterceptorsDisabled = false;
		}
	}
}
