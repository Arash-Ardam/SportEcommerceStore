using SharedKernel;

namespace WalletDomain.Events;

public record WalletOpenedEvent(Guid WalletId, string OwnerId) : AggregateEvent;

