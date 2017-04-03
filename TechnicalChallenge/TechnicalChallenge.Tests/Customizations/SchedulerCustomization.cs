using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Vanilla.Mapping;
using Ploeh.AutoFixture;
using SimpleInjector;
using TechnicalChallenge.Mappers;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge.Tests.Customizations
{
	public class SchedulerCustomization<TSchedule> : ICustomization
		where TSchedule : IHoldInformationAboutScheduleInterval
	{
		public void Customize(IFixture fixture)
		{
			var container = new Container();

			// register all implementations of 'IMapper<TInputParameter, IEnumerable<DateTime>>'
			var inputParameterMapperContractType = typeof(IMapper<ScheduleParametersAndPreviousExecutionTime<TSchedule>, DateTime?>);
			var inputParameterMapperImplementionTypes = from type in typeof(TSchedule).Assembly.GetTypes() where type.IsClass && type.GetInterfaces().Contains(inputParameterMapperContractType) select type;
			foreach (var implementationType in inputParameterMapperImplementionTypes)
				container.Register(inputParameterMapperContractType, implementationType, Lifestyle.Singleton);

			// register the scheduler
			container.RegisterSingleton(typeof(Scheduler<TSchedule>), typeof(Scheduler<TSchedule>));
			container.Verify();

			fixture.Register(() => container.GetInstance<Scheduler<TSchedule>>());
		}
	}
}