using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.QueryDecorators.Interfaces
{
	public interface IDecorateQueryHandler<in TQuery, out TResult> : IQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
	}
}
