using SharedKernel;

namespace IdentityDomain.Events
{
	public record UserProfileRegisteredEvent(string UserId) : AggregateEvent;
	
	
}
