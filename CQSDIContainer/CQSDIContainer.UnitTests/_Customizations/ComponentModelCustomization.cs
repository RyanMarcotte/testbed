using System;
using System.Collections.Generic;
using Castle.Core;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests._Customizations
{
	/// <summary>
	/// Customization that registers a <see cref="ComponentModel"/> object.
	/// </summary>
	public class ComponentModelCustomization : ICustomization
	{
		private readonly bool _hasComponentType;
		private readonly Type _componentType;

		public ComponentModelCustomization()
		{
			_hasComponentType = false;
		}

		public ComponentModelCustomization(Type componentType)
		{
			_hasComponentType = true;
			_componentType = componentType;
		}

		public void Customize(IFixture fixture)
		{
			if (_hasComponentType)
				fixture.Register(() => BuildComponentModel(_componentType));
			else
				fixture.Register(() => new ComponentModel());
		}

		public static ComponentModel BuildComponentModel(Type componentType)
		{
			var componentName = new ComponentName(componentType.FullName, false);
			return new ComponentModel(componentName, new[] { componentType }, componentType, new Dictionary<object, object>());
		}
	}
}
