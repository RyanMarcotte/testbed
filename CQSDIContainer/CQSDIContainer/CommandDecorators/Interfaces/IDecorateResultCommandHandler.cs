using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.CommandDecorators.Interfaces
{
	public interface IDecorateResultCommandHandler<in TCommand, TError> : IResultCommandHandler<TCommand, TError>
	{
	}
}
