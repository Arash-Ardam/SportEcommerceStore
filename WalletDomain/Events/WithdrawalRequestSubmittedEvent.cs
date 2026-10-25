using SharedKernel;

namespace WalletDomain.Events;

public record WithdrawalRequestSubmittedEvent(Guid RequestId,Guid WalletId,long Amount,DateTimeOffset RequestedAt) : AggregateEvent;

