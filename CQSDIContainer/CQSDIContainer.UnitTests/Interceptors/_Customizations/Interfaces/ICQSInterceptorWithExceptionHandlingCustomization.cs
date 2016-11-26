using Castle.Core;
using CQSDIContainer.Interceptors;
using Ploeh.AutoFixture;

namespace CQSDIContainer.UnitTests.Interceptors._Customizations.Interfaces
{
	public interface ICQSInterceptorWithExceptionHandlingCustomization<out TInterceptorType> : ICustomization
		where TInterceptorType : CQSInterceptor
	{
		TInterceptorType CreateInterceptorWithComponentModelSet(IFixture fixture, ComponentModel componentModel);
	}
}