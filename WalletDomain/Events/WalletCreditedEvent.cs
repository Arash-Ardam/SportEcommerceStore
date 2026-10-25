using SharedKernel;

namespace WalletDomain.Events
{
	public record WalletCreditedEvent(Guid WalletId,string Reference,long Amount) : AggregateEvent;
}
