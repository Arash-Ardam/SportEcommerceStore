using SharedKernel;
using VenueDomain.Events;

namespace VenueDomain
{
    public class Venue : AggregateRoot<Guid>
    {
        private List<VenueAdmin> _admins = [];
        private List<Facility> _facilities = [];

		public string Name { get; internal set; } = string.Empty;
        public VenueStatus Status { get; private set; }
        public IReadOnlyList<VenueAdmin> Admins => _admins;
        public Address? Address { get; private set; } 
        public GeoLocation? Location { get; private set; }
        public IReadOnlyList<Facility> Facilities => _facilities;

        public static Venue Register(string name,VenueAdmin admin ,Address address, GeoLocation location)
        {
            if(name.Length > 50) throw VenueErrors.InvalidVenueName();

            return new Venue
			{
				Id = Guid.NewGuid(),
				Name = name,
				Status = VenueStatus.Active,
				_admins = new List<VenueAdmin> { admin },
				Address = address,
				Location = location
			};
		}

        public void UpdateProfile(string name, Address address, GeoLocation location)
        {
            if(name.Length > 50) throw VenueErrors.InvalidVenueName();
            Name = name;
            Address = address;
            Location = location;

            Raise(new VenueProfileUpdatedEvent(base.Id));
        }

        public void Relocate(GeoLocation newLocation,Address address,string description)
        {
            Location = newLocation;
            Address = address;
            Raise(new VenueRelocatedEvent(base.Id, description));
        }

        public void AddFacility(Facility facility)
		{
            if(Facilities.Any(f => f.Title == facility.Title)) throw VenueErrors.DuplicateFacility(facility.Title);

			_facilities.Add(facility);
			Raise(new FaclilitiesChangedEvent(base.Id));
		}

        public void RemoveFacility(Facility facility)
        {
			if (!Facilities.Any(f => f.Title == facility.Title)) throw VenueErrors.FacilityNotFound(facility.Title);

			_facilities.Remove(facility);
			Raise(new FaclilitiesChangedEvent(base.Id));
		}

        public void AddAdmin(VenueAdmin admin)
		{
			if (Admins.Any(a => a.UserId == admin.UserId)) throw VenueErrors.DuplicateAdmin(admin.UserId);

			_admins.Add(admin);
		}

        public void RemoveAdmin(VenueAdmin admin)
        {
			if (!Admins.Any(a => a.UserId == admin.UserId)) throw VenueErrors.AdminNotFound(admin.UserId);
            if(Admins.Count <= 1) throw VenueErrors.AdminRemoveLimit();

			_admins.Remove(admin);
		}

        public void Suspend()
        {
            Status = VenueStatus.Suspended;
			Raise(new VenueStatusChangedEvent(base.Id, VenueStatus.Suspended));
		}

        public void Activate()
		{
			Status = VenueStatus.Active;
			Raise(new VenueStatusChangedEvent(base.Id, VenueStatus.Active));
		}

	}
}
