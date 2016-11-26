using System.Threading;
using System.Threading.Tasks;
using IQ.Platform.Framework.Common.CQS;

namespace IQ.CQS.Lab.Queries
{
	public class GetStringAsyncQueryHandler : IAsyncQueryHandler<GetStringAsyncQuery, string>
	{
		public async Task<string> HandleAsync(GetStringAsyncQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			return await Task.Run(() => "this is the result of an async query handler", cancellationToken);
		}
	}
}
