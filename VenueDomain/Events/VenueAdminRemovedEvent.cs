using SharedKernel;

namespace VenueDomain.Events
{
    public record VenueAdminRemovedEvent(Guid VenueId, string UserId) : AggregateEvent;
}
