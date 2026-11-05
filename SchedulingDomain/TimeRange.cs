namespace SchedulingDomain
{
	public record TimeRange
	{
		public TimeOnly Start { get; private set; }
		public TimeOnly End { get; private set; }

		private TimeRange() { }

		public static TimeRange Set(TimeOnly start, TimeOnly end)
		{
			if (start >= end) throw WeeklyScheduleErrors.InvalidRange();

			return new TimeRange
			{
				Start = start,
				End = end
			};
		}
	}
}
