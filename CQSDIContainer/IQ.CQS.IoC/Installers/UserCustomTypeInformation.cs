using IQ.CQS.IoC.Contributors.Enums;

namespace IQ.CQS.IoC.Installers
{
	/// <summary>
	/// Holds information about a pending user component registration.
	/// </summary>
	internal class UserCustomTypeInformation
	{
		public UserCustomTypeInformation(ServiceRegistrationType serviceRegistrationType, LifestyleType lifestyle)
		{
			ServiceRegistrationType = serviceRegistrationType;
			Lifestyle = lifestyle;
		}

		public ServiceRegistrationType ServiceRegistrationType { get; }
		public LifestyleType Lifestyle { get; }
	}
}