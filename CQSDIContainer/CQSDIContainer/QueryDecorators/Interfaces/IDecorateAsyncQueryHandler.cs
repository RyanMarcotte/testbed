using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.QueryDecorators.Interfaces
{
	public interface IDecorateAsyncQueryHandler<in TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
	}
}
