using SharedKernel;

namespace VenueDomain.Events
{
    // لیست کامل امکانات بعد از تغییر، تا فیلتر جستجو به‌روز شود
    public record FacilitiesChangedEvent(
        Guid VenueId,
        IReadOnlyList<string> FacilityTitles) : AggregateEvent;
}
