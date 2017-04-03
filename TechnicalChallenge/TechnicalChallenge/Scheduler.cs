using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Parameters.Interfaces;

namespace TechnicalChallenge
{
	public class Scheduler<TInputParameter>
		where TInputParameter : IScheduleParameters<TInputParameter>
	{
		private readonly IMapper<TInputParameter, DateTime?> _mapper;

		public Scheduler(IMapper<TInputParameter, DateTime?> mapper)
		{
			_mapper = mapper;
		}

		public DateTime? GetNextExecuteDate(DateTime previousExecutionTimeUtc, TInputParameter parameters)
		{
			DateTime? result = null;
			var newParameters = parameters.WithNewPreviousExecutionTime(previousExecutionTimeUtc);
			result = _mapper.Map(newParameters);

			return (parameters.StopDate != null) && (result != null)
				? result.Value > parameters.StopDate.Value ? null : result
				: result;
		}
	}
}
