using Castle.DynamicProxy;
// ReSharper disable InconsistentNaming

namespace CQSDIContainer.UnitTests._TestUtilities
{
	/// <summary>
	/// Indicates the type of CQS handler method call represented by a fake <see cref="IInvocation"/> object.
	/// </summary>
	public enum CQSHandlerType
	{
		Query,
		AsyncQuery,
		Command,
		ResultCommand_Succeeds,
		ResultCommand_Fails,
		AsyncCommand,
		AsyncResultCommand_Succeeds,
		AsyncResultCommand_Fails
	}
}