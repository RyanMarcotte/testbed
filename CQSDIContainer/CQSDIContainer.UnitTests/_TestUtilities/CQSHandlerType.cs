using Castle.DynamicProxy;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.TestUtilities
{
	/// <summary>
	/// Indicates the type of CQS handler method call represented by a fake <see cref="IInvocation"/> object.
	/// </summary>
	public enum CQSHandlerType
	{
		Query,
		AsyncQuery,
		Command,
		ResultCommand,
		AsyncCommand,
		AsyncResultCommand
	}
}