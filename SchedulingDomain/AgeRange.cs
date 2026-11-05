namespace SchedulingDomain
{
	public record AgeRange
	{
		public int Min { get; private set; }
		public int Max { get; private set; }
		private AgeRange() { }

		public static AgeRange Set(int min, int max)
		{
			if (min <= 0 || max <= 0) throw WeeklyScheduleErrors.InvalidAgeRange();
			if (min >= max) throw WeeklyScheduleErrors.InvalidAgeRange();

			return new AgeRange
			{
				Min = min,
				Max = max
			};
		}
	}
}
