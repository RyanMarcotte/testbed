using System;

namespace TechnicalChallenge.Parameters.Interfaces
{
	public interface IScheduleParameters<out TParameter>
	{
		TParameter WithNewStartDate(DateTime result);
	}
}