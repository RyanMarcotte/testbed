using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSDIContainer.Interceptors.Session.Interfaces
{
	public interface ICQSHandlerSession
	{
		Guid ID { get; }
		bool InterceptorsDisabled { get; }
	}

	public interface ICQSHandlerSessionWithAbilityToDisableInterceptors : ICQSHandlerSession
	{
		new bool InterceptorsDisabled { get; set; }
	}
}
