namespace SchedulingDomain
{
	public record PricingPolicy
	{
		public long BasePrice { get; private set; }
		public long LastMinutePrice { get; private set; }

		private PricingPolicy() { }

		public static PricingPolicy Set(long basePrice, long lastMinutePrice)
		{
			if (basePrice <= 0 || lastMinutePrice <= 0) throw WeeklyScheduleErrors.InvalidAmount();

			return new PricingPolicy
			{
				BasePrice = basePrice,
				LastMinutePrice = lastMinutePrice
			};
		}

	}
	
}
