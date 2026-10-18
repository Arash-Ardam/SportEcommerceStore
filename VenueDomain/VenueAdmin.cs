using SharedKernel;

namespace VenueDomain
{
    public class VenueAdmin : Entity<Guid>
    {
        public string UserId { get; private set; } = string.Empty;
        public DateTimeOffset AddedAt { get; private set; }

        private VenueAdmin() { }

        // internal: فقط Venue (داخل همین پروژه) می‌تواند ادمین بسازد
        internal static VenueAdmin Create(string userId, DateTimeOffset addedAt)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw VenueErrors.InvalidAdminUserId();

            return new VenueAdmin
            {
                Id = Guid.NewGuid(),
                UserId = userId.Trim(),
                AddedAt = addedAt
            };
        }
    }
}
