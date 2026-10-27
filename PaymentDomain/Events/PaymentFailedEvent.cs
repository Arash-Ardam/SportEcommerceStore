using SharedKernel;
namespace PaymentDomain.Events;

public record PaymentFailedEvent   (Guid PaymentId,Guid BookingId, string UserId, string Reason, DateTimeOffset Time) : AggregateEvent;
