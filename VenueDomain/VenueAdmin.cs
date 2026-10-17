using SharedKernel;

namespace VenueDomain
{
    public class VenueAdmin : Entity<Guid>
    {
        public string UserId { get; internal set; }
        public DateTimeOffset AddedAt { get; internal set; }

        private VenueAdmin()
        {
            
        }

        public static VenueAdmin Build(string userId,DateTimeOffset addedAt)
            => new VenueAdmin
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AddedAt = addedAt
            };

    }
}
