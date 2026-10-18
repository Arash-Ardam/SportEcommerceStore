using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueAdminAddedEvent(Guid VenueId, string UserId) : AggregateEvent;
}
