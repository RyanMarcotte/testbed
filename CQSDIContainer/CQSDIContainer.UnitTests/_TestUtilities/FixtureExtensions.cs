using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests._TestUtilities
{
	/// <summary>
	/// Extension methods for the <see cref="IFixture"/> interface.
	/// </summary>
	internal static class FixtureExtensions
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
