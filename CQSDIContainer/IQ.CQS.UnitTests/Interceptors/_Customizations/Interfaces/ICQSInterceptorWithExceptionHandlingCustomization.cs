using Castle.Core;
using IQ.CQS.Interceptors;
using Ploeh.AutoFixture;

namespace IQ.CQS.UnitTests.Interceptors._Customizations.Interfaces
{
	public interface ICQSInterceptorWithExceptionHandlingCustomization<out TInterceptorType> : ICustomization
		where TInterceptorType : CQSInterceptor
	{
		TInterceptorType CreateInterceptorWithComponentModelSet(IFixture fixture, ComponentModel componentModel);
	}
}