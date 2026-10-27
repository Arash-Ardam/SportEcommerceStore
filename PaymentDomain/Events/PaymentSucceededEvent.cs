using SharedKernel;
namespace PaymentDomain.Events;

public record PaymentSucceededEvent(Guid PaymentId,Guid BookingId, string UserId, long Amount, string TrackingCode, DateTimeOffset CompletedAt) : AggregateEvent;
