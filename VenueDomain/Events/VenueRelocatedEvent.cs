using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueRelocatedEvent(
        Guid VenueId,
        string description) : AggregateEvent;

}
