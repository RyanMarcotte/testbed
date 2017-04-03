using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Mappers;
using TechnicalChallenge.Parameters;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge
{
	public class Scheduler<TSchedule>
		where TSchedule : IHoldInformationAboutScheduleInterval
	{
		private readonly IMapper<ScheduleParametersAndPreviousExecutionTime<TSchedule>, DateTime?> _mapper;

		public Scheduler(IMapper<ScheduleParametersAndPreviousExecutionTime<TSchedule>, DateTime?> mapper)
		{
			_mapper = mapper;
		}

		public DateTime? GetNextExecuteDate(TSchedule schedule, DateTime? previousExecutionTime)
		{
			// start search at immediately after previous execution time
			// if not previously executed, then start search immediately before the schedule starts
			var searchStart = previousExecutionTime?.AddMilliseconds(1) ?? schedule.StartDate - TimeSpan.FromMilliseconds(1);
			var result = _mapper.Map(new ScheduleParametersAndPreviousExecutionTime<TSchedule>(schedule, searchStart));
			return (schedule.StopDate != null) && (result != null)
				? result.Value > schedule.StopDate.Value ? null : result
				: result;
		}
	}
}
