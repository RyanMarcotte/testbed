using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Interceptors.Session.Interfaces;

namespace CQSDIContainer.Interceptors.Session
{
	public class CQSHandlerSession : ICQSHandlerSessionWithAbilityToDisableInterceptors
	{
		public Guid ID { get; } = Guid.NewGuid();
		public bool InterceptorsDisabled { get; set; }
	}
}
