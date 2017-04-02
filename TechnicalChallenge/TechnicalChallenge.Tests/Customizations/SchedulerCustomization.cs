using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.Mapping;
using Ploeh.AutoFixture;
using SimpleInjector;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Tests.Customizations
{
	public class SchedulerCustomization<TInputParameter> : ICustomization
		where TInputParameter : IScheduleParameters<TInputParameter>
	{
		public void Customize(IFixture fixture)
		{
			var container = new Container();

			// register all implementations of 'IMapper<TInputParameter, IEnumerable<DateTime>>'
			var inputParameterMapperContractType = typeof(IMapper<TInputParameter, DateTime?>);
			var inputParameterMapperImplementionTypes = from type in typeof(TInputParameter).Assembly.GetTypes() where type.IsClass && type.GetInterfaces().Contains(inputParameterMapperContractType) select type;
			foreach (var implementationType in inputParameterMapperImplementionTypes)
				container.Register(inputParameterMapperContractType, implementationType, Lifestyle.Singleton);

			// register the scheduler
			container.RegisterSingleton(typeof(Scheduler<TInputParameter>), typeof(Scheduler<TInputParameter>));
			container.Verify();

			fixture.Register(() => container.GetInstance<Scheduler<TInputParameter>>());
		}
	}
}