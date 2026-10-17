using SharedKernel;

namespace VenueDomain.Events
{
	public record VenueStatusChangedEvent(Guid venueId, VenueStatus newStatus) : AggregateEvent;
	
}
