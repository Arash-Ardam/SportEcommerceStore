using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueStatusChangedEvent(Guid VenueId, VenueStatus NewStatus) : AggregateEvent;
}
