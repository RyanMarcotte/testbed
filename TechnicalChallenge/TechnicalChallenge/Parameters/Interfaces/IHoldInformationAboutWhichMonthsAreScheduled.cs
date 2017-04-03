using System;
using TechnicalChallenge.Models;

namespace TechnicalChallenge.Parameters.Interfaces
{
	public interface IHoldInformationAboutWhichMonthsAreScheduled
	{
		bool ScheduledForJanuary { get; }
		bool ScheduledForFebruary { get; }
		bool ScheduledForMarch { get; }
		bool ScheduledForApril { get; }
		bool ScheduledForMay { get; }
		bool ScheduledForJune { get; }
		bool ScheduledForJuly { get; }
		bool ScheduledForAugust { get; }
		bool ScheduledForSeptember { get; }
		bool ScheduledForOctober { get; }
		bool ScheduledForNovember { get; }
		bool ScheduledForDecember { get; }
	}

	// ReSharper disable once InconsistentNaming
	public static class ExtensionsFor_IHoldInformationAboutWhichMonthsAreScheduled
	{
		public static MonthAndYear GetNextScheduledMonthAndYear(this IHoldInformationAboutWhichMonthsAreScheduled source, DateTime dateTime)
		{
			var nextMonth = dateTime.AddMonths(1);
			while (!(source.ScheduledForJanuary && (nextMonth.Month == 1))
					&& !(source.ScheduledForFebruary && (nextMonth.Month == 2))
					&& !(source.ScheduledForMarch && (nextMonth.Month == 3))
					&& !(source.ScheduledForApril && (nextMonth.Month == 4))
					&& !(source.ScheduledForMay && (nextMonth.Month == 5))
					&& !(source.ScheduledForJune && (nextMonth.Month == 6))
					&& !(source.ScheduledForJuly && (nextMonth.Month == 7))
					&& !(source.ScheduledForAugust && (nextMonth.Month == 8))
					&& !(source.ScheduledForSeptember && (nextMonth.Month == 9))
					&& !(source.ScheduledForOctober && (nextMonth.Month == 10))
					&& !(source.ScheduledForNovember && (nextMonth.Month == 11))
					&& !(source.ScheduledForDecember && (nextMonth.Month == 12)))
			{
				nextMonth = nextMonth.AddMonths(1);
			}

			var nextMonthAndYear = nextMonth.ToMonthAndYear();
			return nextMonthAndYear;
		}
	}
}