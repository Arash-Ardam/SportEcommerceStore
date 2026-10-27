using SharedKernel;
namespace PaymentDomain.Events;

public record PaymentInitiatedEvent(Guid PaymentId,Guid BookingId, string UserId,PaymentMethod Method, DateTimeOffset CreatedAt) : AggregateEvent;
