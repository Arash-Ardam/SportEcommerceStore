using SharedKernel;

namespace WalletDomain.Events;

public record WithdrawalRequestRejectedEvent(Guid RequestId, Guid WalletId, long Amount, DateTimeOffset RequestedAt,string Reason) : AggregateEvent;

