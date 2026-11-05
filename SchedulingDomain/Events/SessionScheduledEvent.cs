using SharedKernel;

namespace SchedulingDomain.Events
{
	public record SessionScheduledEvent(Guid SessionId, Guid SlotId, DateOnly Date,int capacity,AdmissionPolicy admissionPolicy,PricingPolicy PricingPolicy) : AggregateEvent;
	public record SessionPricingChangedEvent(Guid SessionId) : AggregateEvent;
	public record SessionCancelledEvent(Guid SessionId,DateTimeOffset cancellationDate,string reason) : AggregateEvent;

}
