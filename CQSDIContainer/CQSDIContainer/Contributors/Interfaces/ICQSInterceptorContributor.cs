using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace CQSDIContainer.Contributors.Interfaces
{
	/// <summary>
	/// Marker interface used by Castle.Windsor for registering CQS-interceptor contributor classes.
	/// </summary>
	public interface ICQSInterceptorContributor : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Indicates if the contributor is managing the application of interceptors to nested CQS handlers.
		/// </summary>
		bool IsContributingToComponentModelConstructionForNestedCQSHandlers { get; }

		/// <summary>
		/// Indicates which types of handlers to apply the interceptor to.
		/// </summary>
		InterceptorUsageOptions HandlerTypesToApplyTo { get; }
	}
}