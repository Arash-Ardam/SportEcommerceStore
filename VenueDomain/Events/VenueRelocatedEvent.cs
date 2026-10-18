using SharedKernel;

namespace VenueDomain.Events
{
    // جستجوی «نزدیک‌ترین باشگاه» مختصات جدید را لازم دارد
    public record VenueRelocatedEvent(
        Guid VenueId,
        Address Address,
        GeoLocation Location) : AggregateEvent;
}
