using Castle.MicroKernel;
using FakeItEasy;
using Ploeh.AutoFixture;

namespace IQ.CQS.UnitTests.Framework.Customizations
{
	/// <summary>
	/// Customization that freezes an instance of <see cref="IKernel"/> that does nothing.
	/// </summary>
	public class NullKernelCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var kernel = A.Fake<IKernel>();
			fixture.Freeze(kernel);
		}
	}
}