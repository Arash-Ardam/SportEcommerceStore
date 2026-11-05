using SharedKernel;

namespace SchedulingDomain
{
	public class ScheduleSlot : Entity<Guid>
	{
		public DayOfWeek Day { get; private set; }
		public TimeRange? TimeRange { get; private set; }
		public SessionType Type { get; private set; }
		public int Capacity { get; private set; }
		public AdmissionPolicy? AdmissionPolicy { get; private set; }
		public PricingPolicy? PricingPolicy { get; private set; }


		public static ScheduleSlot Create(DayOfWeek day, TimeRange range, SessionType type,int capacity, AdmissionPolicy admissionPolicy, PricingPolicy pricingPolicy)
		{
			if (capacity <= 0) throw WeeklyScheduleErrors.InvalidCapacity();

			return new ScheduleSlot
			{
				Id = Guid.NewGuid(),
				Day = day,
				TimeRange = range,
				Type = type,
				Capacity = capacity,
				AdmissionPolicy = admissionPolicy,
				PricingPolicy = pricingPolicy
			};
		}

		public void Update(DayOfWeek day, TimeRange range, SessionType type, int capacity, AdmissionPolicy admissionPolicy, PricingPolicy pricingPolicy)
		{
			if (capacity <= 0) throw WeeklyScheduleErrors.InvalidCapacity();

			Day = day;
			TimeRange = range;
			Type = type;
			Capacity = capacity;
			AdmissionPolicy = admissionPolicy;
			PricingPolicy = pricingPolicy;
		}
	}
	
}
