using SharedKernel;
using VenueDomain.Events;

namespace VenueDomain
{
    public class Venue : AggregateRoot<Guid>
    {
        public const int NameMaxLength = 50;

        private readonly List<VenueAdmin> _admins = [];
        private readonly List<Facility> _facilities = [];

        public string Name { get; private set; } = string.Empty;
        public VenueStatus Status { get; private set; }
        public IReadOnlyList<VenueAdmin> Admins => _admins;
        public Address Address { get; private set; } = null!;
        public GeoLocation Location { get; private set; } = null!;
        public IReadOnlyList<Facility> Facilities => _facilities;

        private Venue() { }

        // ادمین اول فقط با userId می‌آید؛ خود Venue شیء VenueAdmin را می‌سازد
        public static Venue Register(string name, string firstAdminUserId, Address address, GeoLocation location, DateTimeOffset now)
        {
            ValidateName(name);
            if (address is null) throw VenueErrors.InvalidAddress();
            if (location is null) throw VenueErrors.InvalidLocation();

            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Status = VenueStatus.Active,
                Address = address,
                Location = location
            };

            venue._admins.Add(VenueAdmin.Create(firstAdminUserId, now));

            venue.Raise(new VenueCreatedEvent(venue.Id, venue.Name, address, location));
            return venue;
        }

        // فقط اسم؛ تغییر مکان فقط از Relocate انجام می‌شود
        public void UpdateProfile(string name)
        {
            ValidateName(name);
            if (name.Trim() == Name) return;

            Name = name.Trim();
            Raise(new VenueProfileUpdatedEvent(Id, Name));
        }

        public void Relocate(GeoLocation newLocation, Address newAddress)
        {
            if (newAddress is null) throw VenueErrors.InvalidAddress();
            if (newLocation is null) throw VenueErrors.InvalidLocation();

            // record: مقایسه بر اساس مقدار. اگر چیزی عوض نشده، رویدادی هم لازم نیست
            if (newLocation == Location && newAddress == Address) return;

            Location = newLocation;
            Address = newAddress;
            Raise(new VenueRelocatedEvent(Id, newAddress, newLocation));
        }

        public void AddFacility(Facility facility)
        {
            if (facility is null) throw VenueErrors.InvalidFacilityTitle();
            if (_facilities.Any(f => f.Title == facility.Title)) throw VenueErrors.DuplicateFacility(facility.Title);

            _facilities.Add(facility);
            Raise(new FacilitiesChangedEvent(Id, _facilities.Select(f => f.Title).ToList()));
        }

        public void RemoveFacility(string title)
        {
            var target = _facilities.FirstOrDefault(f => f.Title == title)
                ?? throw VenueErrors.FacilityNotFound(title);

            _facilities.Remove(target);
            Raise(new FacilitiesChangedEvent(Id, _facilities.Select(f => f.Title).ToList()));
        }

        public void AddAdmin(string userId, DateTimeOffset now)
        {
            if (_admins.Any(a => a.UserId == userId)) throw VenueErrors.DuplicateAdmin(userId);

            _admins.Add(VenueAdmin.Create(userId, now));
            Raise(new VenueAdminAddedEvent(Id, userId));
        }

        public void RemoveAdmin(string userId)
        {
            var target = _admins.FirstOrDefault(a => a.UserId == userId)
                ?? throw VenueErrors.AdminNotFound(userId);

            if (_admins.Count <= 1) throw VenueErrors.AdminRemoveLimit();

            _admins.Remove(target);
            Raise(new VenueAdminRemovedEvent(Id, userId));
        }

        public void Suspend()
        {
            if (Status == VenueStatus.Suspended)
                throw VenueErrors.InvalidVenueStatusTransition(Status, VenueStatus.Suspended);

            Status = VenueStatus.Suspended;
            Raise(new VenueStatusChangedEvent(Id, VenueStatus.Suspended));
        }

        public void Activate()
        {
            if (Status == VenueStatus.Active)
                throw VenueErrors.InvalidVenueStatusTransition(Status, VenueStatus.Active);

            Status = VenueStatus.Active;
            Raise(new VenueStatusChangedEvent(Id, VenueStatus.Active));
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw VenueErrors.NullVenueName();
            if (name.Trim().Length > NameMaxLength) throw VenueErrors.InvalidVenueName();
        }
    }
}
