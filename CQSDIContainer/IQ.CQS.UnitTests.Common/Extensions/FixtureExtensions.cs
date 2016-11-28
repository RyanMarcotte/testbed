using System.Linq;
using Ploeh.AutoFixture;

namespace IQ.CQS.UnitTests.Framework.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="IFixture"/> interface.
	/// </summary>
	public static class FixtureExtensions
	{
		/// <summary>
		/// Applies a set of customizations.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		/// <param name="customizations">The customization to apply.</param>
		/// <returns></returns>
		public static IFixture CustomizeWithMany(this IFixture fixture, params ICustomization[] customizations)
		{
			return customizations.Aggregate(fixture, (current, customization) => current.Customize(customization));
		}
	}
}
