using SharedKernel;

namespace WalletDomain.Events;

public record WithdrawalRequestPaidEvent(Guid RequestId, Guid WalletId, long Amount, DateTimeOffset RequestedAt) : AggregateEvent;

