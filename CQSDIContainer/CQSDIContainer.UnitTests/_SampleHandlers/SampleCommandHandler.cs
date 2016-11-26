using CQSDIContainer.UnitTests._SampleHandlers.Parameters;
using IQ.Platform.Framework.Common.CQS;

namespace CQSDIContainer.UnitTests._SampleHandlers
{
	public class SampleCommandHandler : ICommandHandler<SampleCommand>
	{
		public static readonly object ReturnValue = typeof(void);

		public void Handle(SampleCommand command)
		{
		}
	}
}