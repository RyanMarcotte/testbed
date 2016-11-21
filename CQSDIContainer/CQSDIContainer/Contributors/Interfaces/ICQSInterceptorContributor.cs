using Castle.MicroKernel.ModelBuilder;

namespace CQSDIContainer.Contributors.Interfaces
{
	/// <summary>
	/// Marker interface used by Castle.Windsor for registering CQS-interceptor contributor classes.
	/// </summary>
	public interface ICQSInterceptorContributor : IContributeComponentModelConstruction
	{
	}
}