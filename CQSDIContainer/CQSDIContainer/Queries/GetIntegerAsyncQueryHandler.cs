using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.Queries
{
	public class GetIntegerAsyncQueryHandler : IAsyncQueryHandler<GetIntegerAsyncQuery, int>
	{
		public async Task<int> HandleAsync(GetIntegerAsyncQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => query.Value, cancellationToken);
		}
	}
}
