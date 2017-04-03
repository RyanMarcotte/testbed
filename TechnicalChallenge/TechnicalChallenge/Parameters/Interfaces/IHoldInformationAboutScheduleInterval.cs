using System;

namespace TechnicalChallenge.Parameters.Interfaces
{
	public interface IHoldInformationAboutScheduleInterval
	{
		DateTime StartDate { get; }
		DateTime? StopDate { get; }
	}
}