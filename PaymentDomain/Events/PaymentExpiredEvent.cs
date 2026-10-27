using SharedKernel;
namespace PaymentDomain.Events;

public record PaymentExpiredEvent  (Guid PaymentId,Guid BookingId, string UserId, DateTimeOffset Time) : AggregateEvent;