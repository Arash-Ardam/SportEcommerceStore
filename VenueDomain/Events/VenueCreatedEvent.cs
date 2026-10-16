using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueCreatedEvent(
        Guid venueId,
        string name
        ) : AggregateEvent;
    
}
