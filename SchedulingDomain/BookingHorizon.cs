namespace SchedulingDomain
{
	public record BookingHorizon
	{
		public int Value { get; private set; }

		private BookingHorizon() { }

		public static BookingHorizon Set(int value)
		{
			if (value < 1 || value > 30) throw WeeklyScheduleErrors.InvalidHorizon();

			return new BookingHorizon { Value = value };
		}
	}
	
}
