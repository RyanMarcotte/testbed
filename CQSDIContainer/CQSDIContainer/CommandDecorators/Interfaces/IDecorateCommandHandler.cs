using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.CommandDecorators.Interfaces
{
	/// <summary>
	/// Marker interface 
	/// </summary>
	/// <typeparam name="TCommand"></typeparam>
	public interface IDecorateCommandHandler<in TCommand> : ICommandHandler<TCommand>
	{
	}
}
