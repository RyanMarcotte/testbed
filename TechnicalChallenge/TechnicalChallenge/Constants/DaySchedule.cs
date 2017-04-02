﻿using System;

namespace TechnicalChallenge.Constants
{
	[Flags]
	public enum DaySchedule
	{
		None = 0,
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesday = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64
	}
}