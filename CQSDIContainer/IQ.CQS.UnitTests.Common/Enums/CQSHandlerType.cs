using Castle.DynamicProxy;

// ReSharper disable InconsistentNaming

namespace IQ.CQS.UnitTests.Framework.Enums
{
	/// <summary>
	/// Indicates the type of CQS handler method call represented by a fake <see cref="IInvocation"/> object.
	/// </summary>
	public enum CQSHandlerType
	{
		Query_ReturnsValueType,
		Query_ReturnsReferenceType,
		AsyncQuery_ReturnsValueType,
		AsyncQuery_ReturnsReferenceType,
		Command,
		ResultCommand_Succeeds,
		ResultCommand_Fails,
		AsyncCommand,
		AsyncResultCommand_Succeeds,
		AsyncResultCommand_Fails
	}
}