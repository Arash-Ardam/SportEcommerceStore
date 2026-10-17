using SharedKernel;

namespace VenueDomain.Events
{
    public record FaclilitiesChangedEvent(Guid VenueId) : AggregateEvent;

}
