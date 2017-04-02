using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Parameters;

namespace TechnicalChallenge
{
	public class Scheduler<TInputParameter>
	{
		private readonly IMapper<TInputParameter, InternalSchedulerInputParameters> _mapper;

		public Scheduler(IMapper<TInputParameter, InternalSchedulerInputParameters> mapper)
		{
			_mapper = mapper;
		}

		public DateTime GetNextExecuteDate(DateTime previousExecutionTimeUtc, TInputParameter parameters)
		{
			throw new NotImplementedException();
		}
	}

	public class InternalSchedulerInputParameters
	{

	}
}
