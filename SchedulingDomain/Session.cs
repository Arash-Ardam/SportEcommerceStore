using SchedulingDomain.Events;
using SharedKernel;

namespace SchedulingDomain
{
	public class Session : AggregateRoot<Guid>
	{
		public Guid VenueId { get; private set; }
		public Guid SourceSlotId { get; private set; }
		public DateOnly Date { get; private set; }
		public TimeRange? TimeRange { get; private set; } 
		public SessionType Type { get; private set; }
		public AdmissionPolicy? AdmissionPolicy { get; private set; } 
		public PricingPolicy? PricingPolicy { get; private set; } 
		public int InitialCapacity { get; private set; }
		public SessionStatus Status { get; private set; }
		public string? CancellationReason { get; private set; }

		private Session() { }

		public static Session FromSlot(Guid venueId, ScheduleSlot slot, DateOnly date)
		{
			if (date.DayOfWeek != slot.Day) throw SessionErrors.InvalidDate();
			if (venueId == Guid.Empty) throw SessionErrors.InvalidVenueId();

			var session = new Session
			{
				Id = Guid .NewGuid(),
				VenueId = venueId,
				SourceSlotId = slot.Id,
				Date = date,
				TimeRange = slot.TimeRange,
				Type =slot.Type,
				AdmissionPolicy = slot.AdmissionPolicy,
				PricingPolicy = slot.PricingPolicy,
				InitialCapacity = slot.Capacity,
				Status = SessionStatus.Scheduled
			};

			session.Raise(new SessionScheduledEvent(session.Id, slot.Id,session.Date,slot.Capacity,session.AdmissionPolicy,session.PricingPolicy));
			return session;
		}

		public void ChangePricing(PricingPolicy pricing, DateTimeOffset now)
		{
			if (Status != SessionStatus.Scheduled) throw SessionErrors.InvalidPricingStatus();

			PricingPolicy = pricing;
			Raise(new SessionPricingChangedEvent(Id));
		}

		public void Cancel(string reason, DateTimeOffset now)
		{
			var startDateTime = Date.ToDateTime(TimeRange?.Start ?? throw SessionErrors.InvalidDate());
			var endDateTime = Date.ToDateTime(TimeRange?.End ?? throw SessionErrors.InvalidDate());

			if (now >= startDateTime && now <= endDateTime) throw SessionErrors.InvalidCancellationTime();
			if (Status == SessionStatus.Cancelled) throw SessionErrors.DuplicateCancellation();
			if (string.IsNullOrWhiteSpace(reason)) throw SessionErrors.InvalidCancellationReason();

			Status = SessionStatus.Cancelled;
			CancellationReason = reason;
			Raise(new SessionCancelledEvent(Id,now,reason));
		}

	}
}
