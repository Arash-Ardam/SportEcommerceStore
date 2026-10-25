using SharedKernel;

namespace WalletDomain.Events
{
	public record WalletDebitedEvent(Guid WalletId, string Reference, long Amount) : AggregateEvent;
}
