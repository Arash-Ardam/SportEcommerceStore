using SharedKernel;

namespace IdentityDomain.Events
{
	public record UserProfileChangedEvent(string UserId) : AggregateEvent;
	
}
