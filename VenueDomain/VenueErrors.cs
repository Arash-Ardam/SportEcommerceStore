using SharedKernel;

namespace VenueDomain
{
    public static class VenueErrors
    {
        public static DomainException InvalidVenueName() =>
            new DomainException
            {
                Code = "INVALID_VENUE_NAME",
                Description = $"عنوان نباید بیشتر از 50 کاراکتر باشد."
            };

        public static DomainException VenueNotFound(Guid venueId) =>
            new DomainException
            {
                Code = "VENUE_NOT_FOUND",
                Description = $"Venue with ID '{venueId}' was not found."
            };
        public static DomainException InvalidVenueStatusTransition(VenueStatus currentStatus, VenueStatus newStatus) =>
            new DomainException
            {
                Code = "INVALID_VENUE_STATUS_TRANSITION",
                Description = $"Cannot transition venue status from '{currentStatus}' to '{newStatus}'."
            };
        public static DomainException VenueAlreadyExists(string name) =>
            new DomainException
            {
                Code = "VENUE_ALREADY_EXISTS",
                Description = $"A venue with the name '{name}' already exists."
            };
    }
}
