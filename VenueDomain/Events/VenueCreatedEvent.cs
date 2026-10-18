using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueCreatedEvent(
        Guid VenueId,
        string Name,
        Address Address,
        GeoLocation Location) : AggregateEvent;
}
