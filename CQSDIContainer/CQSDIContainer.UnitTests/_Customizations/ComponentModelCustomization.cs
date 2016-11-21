using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Ploeh.AutoFixture;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Customizations
{
	/// <summary>
	/// Customization that registers a <see cref="ComponentModel"/> object.
	/// </summary>
	public class ComponentModelCustomization : ICustomization
	{
		private readonly Type _componentType;

		public ComponentModelCustomization(Type componentType)
		{
			_componentType = componentType;
		}

		public void Customize(IFixture fixture)
		{
			var componentName = new ComponentName(_componentType.FullName, false);
			var componentModel = new ComponentModel(componentName, _componentType.GetInterfaces(), _componentType, new Dictionary<object, object>());
			fixture.Register(() => componentModel);
		}
	}
}
