using SharedKernel;
using VenueDomain.Events;

namespace VenueDomain
{
    public class Venue : AggregateRoot<Guid>
    {
        public string Name { get; internal set; } = string.Empty;
        public VenueStatus Status { get; internal set; }
        public List<VenueAdmin> Admins { get; internal set; } = new();
        public Address Address { get; internal set; } = new();
        public GeoLocation Location { get; internal set; } = new();
        public List<Facility> Facilities { get; internal set; } = new();

        public Venue Register(string name,VenueAdmin admin ,Address address, GeoLocation location)
        {
            if(name.Length > 50) throw VenueErrors.InvalidVenueName();

            Admins.Add(admin);
            Name = name;
            Address = address;
            Location = location;
            Raise(new VenueCreatedEvent(base.Id, name));

            return this;
        }

        public void UpdateProfile(string name, Address address, GeoLocation location)
        {
            if(name.Length > 50) throw VenueErrors.InvalidVenueName();
            Name = name;
            Address = address;
            Location = location;
        }

    }
}
