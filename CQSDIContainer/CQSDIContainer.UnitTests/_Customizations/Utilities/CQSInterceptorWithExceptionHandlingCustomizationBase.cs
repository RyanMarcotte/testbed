using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSDIContainer.Interceptors;
using Ploeh.AutoFixture;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Customizations.Utilities
{
	public interface ICQSInterceptorWithExceptionHandlingCustomization<out TInterceptorType> : ICustomization
		where TInterceptorType : CQSInterceptor
	{
		TInterceptorType CreateInterceptor(IFixture fixture);
	}

	public abstract class CQSInterceptorWithExceptionHandlingCustomizationBase<TInterceptorType> : ICQSInterceptorWithExceptionHandlingCustomization<TInterceptorType>
		where TInterceptorType : CQSInterceptor
	{
		/// <summary>
		/// Create an instance of the interceptor you are testing.
		/// </summary>
		/// <param name="fixture">The fixture.  If non-null, you are in the customization stage and can register any additional dependencies required for your tests.</param>
		/// <returns></returns>
		public abstract TInterceptorType CreateInterceptor(IFixture fixture);

		public void Customize(IFixture fixture)
		{
			fixture.Register(() => CreateInterceptor(fixture));
		}
	}
}
