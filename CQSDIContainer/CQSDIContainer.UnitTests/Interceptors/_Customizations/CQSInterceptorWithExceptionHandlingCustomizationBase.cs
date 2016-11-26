using Castle.Core;
using CQSDIContainer.Interceptors;
using CQSDIContainer.UnitTests.Interceptors._Customizations.Interfaces;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests.Interceptors._Customizations
{
	/// <summary>
	/// Configures the creation of an interceptor for unit tests.
	/// </summary>
	/// <typeparam name="TInterceptorType">The type of interceptor to create.</typeparam>
	public abstract class CQSInterceptorWithExceptionHandlingCustomizationBase<TInterceptorType> : ICQSInterceptorWithExceptionHandlingCustomization<TInterceptorType>
		where TInterceptorType : CQSInterceptor
	{
		/// <summary>
		/// Perform any registrations of components that are required for your tests.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		protected abstract void RegisterDependencies(IFixture fixture);

		/// <summary>
		/// Create an instance of the interceptor you are testing.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		/// <returns></returns>
		protected abstract TInterceptorType CreateInterceptor(IFixture fixture);

		/// <summary>
		/// Create an instance of the interceptor you are testing and then sets the intercepted component model property for it.
		/// </summary>
		/// <param name="fixture"></param>
		/// <param name="componentModel"></param>
		/// <returns></returns>
		public TInterceptorType CreateInterceptorWithComponentModelSet(IFixture fixture, ComponentModel componentModel)
		{
			var interceptor = CreateInterceptor(fixture);
			interceptor.SetInterceptedComponentModel(componentModel);
			return interceptor;
		}

		/// <summary>
		/// Use your factory method to create an instance of the interceptor you are testing.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		public void Customize(IFixture fixture)
		{
			RegisterDependencies(fixture);
			fixture.Register(() => CreateInterceptorWithComponentModelSet(fixture, fixture.Create<ComponentModel>()));
		}
	}
}
