using SharedKernel;

namespace VenueDomain.Events
{
    public record FaclilitiesChangedEvent(
        Guid VenueId,
        string description) : AggregateEvent;

}
