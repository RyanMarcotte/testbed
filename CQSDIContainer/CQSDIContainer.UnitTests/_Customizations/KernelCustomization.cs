using Castle.MicroKernel;
using FakeItEasy;
using Ploeh.AutoFixture;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Customizations
{
	public class KernelCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register(A.Fake<IKernel>);
		}
	}
}