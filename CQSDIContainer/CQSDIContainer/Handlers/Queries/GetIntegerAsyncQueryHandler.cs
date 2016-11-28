using System.Threading;
using System.Threading.Tasks;
using IQ.Vanilla.CQS;

namespace IQ.CQS.Lab.Handlers.Queries
{
	public class GetIntegerAsyncQueryHandler : IAsyncQueryHandler<GetIntegerAsyncQuery, int>
	{
		public async Task<int> HandleAsync(GetIntegerAsyncQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => query.Value, cancellationToken);
		}
	}
}
