using SharedKernel;

namespace SchedulingDomain
{
	public class WeeklySchedule : AggregateRoot<Guid>
	{
		public Guid VenueId { get; private set; }
		public BookingHorizon? BookingHorizon { get; private set; }
		private readonly List<ScheduleSlot> _slots = [];
		public IReadOnlyList<ScheduleSlot> Slots => _slots;

		private WeeklySchedule() { }

		public static WeeklySchedule Create(Guid venueId,BookingHorizon horizon)
		{
			if (Guid.Empty == venueId) throw WeeklyScheduleErrors.InvalidVenueId();
			if (horizon.Value <= 0) throw WeeklyScheduleErrors.InvalidHorizon();

			return new WeeklySchedule
			{
				Id = Guid.NewGuid(),
				BookingHorizon = horizon,
				VenueId = venueId
			};
		}

		public void AddSlot(DayOfWeek day, TimeRange range, SessionType type, AdmissionPolicy admissionPolicy, PricingPolicy pricingPolicy, int capacity)
		{
			bool isOverLoaded = _slots.Any(slot => slot.Day == day && slot.TimeRange.Start < range.End && slot.TimeRange.End > range.Start);

			if (isOverLoaded) throw WeeklyScheduleErrors.RangeOverloaded();

			var newSlot = ScheduleSlot.Create(day, range, type, capacity, admissionPolicy, pricingPolicy);
			_slots.Add(newSlot);
		}

		public void UpdateSlot(Guid slotId,DayOfWeek day, TimeRange range, SessionType type, AdmissionPolicy admissionPolicy, PricingPolicy pricingPolicy, int capacity)
		{
			if (slotId == Guid.Empty) throw WeeklyScheduleErrors.InvalidSlotId();

			bool isOverLoaded = _slots.Any(slot => slot.Id != slotId && slot.Day == day && ((slot.TimeRange.Start > range.Start && slot.TimeRange.Start < range.End) ||
									   (slot.TimeRange.End < range.End && slot.TimeRange.End > range.Start)));

			if (isOverLoaded) throw WeeklyScheduleErrors.RangeOverloaded();

			var targetSlot = _slots.FirstOrDefault(slot => slot.Id == slotId);

			if (targetSlot == null) throw WeeklyScheduleErrors.InvalidSlotId();

			targetSlot.Update(day, range, type, capacity, admissionPolicy, pricingPolicy);
		}

		public void RemoveSlot(Guid slotId)
		{
			if (slotId == Guid.Empty) throw WeeklyScheduleErrors.InvalidSlotId();

			var targetSlot = _slots.FirstOrDefault(slot => slot.Id == slotId);

			if (targetSlot == null) throw WeeklyScheduleErrors.InvalidSlotId();

			_slots.Remove(targetSlot);
		}

		public void ChangeBookingHorizon(int value)
		{
			BookingHorizon = BookingHorizon.Set(value);
		}

	}
	
}
