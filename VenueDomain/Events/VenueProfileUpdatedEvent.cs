using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueProfileUpdatedEvent(
        Guid VenueId,
        string Name) : AggregateEvent;
}
