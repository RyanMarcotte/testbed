using System;
using System.Collections.Generic;
using IQ.Vanilla.Mapping;
using TechnicalChallenge.Parameters;

namespace TechnicalChallenge.Mappers
{
	/// <summary>
	/// Maps a schedule input set to the next execution time after the start date+time.
	/// </summary>
	public class ScheduleInputParameterSet2FormatMapper : IMapper<ScheduleInputParameterSet2Format, DateTime?>
	{
		public DateTime? Map(ScheduleInputParameterSet2Format source)
		{
			return null;
		}
	}
}