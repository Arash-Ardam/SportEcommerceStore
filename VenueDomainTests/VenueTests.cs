using SharedKernel;
using VenueDomain;
using VenueDomain.Events;

namespace VenueDomainTests
{
	public class VenueTests
	{
		[Fact]
		public void Should_Register_With_Valid_Name()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);

			// Act
			var venue = Venue.Register("My Venue", admin, address, location);

			// Assert
			Assert.Equal("My Venue", venue.Name);
			Assert.Equal(VenueStatus.Active, venue.Status);
			Assert.True(venue.Admins.Contains(admin));
		}

		[Fact]
		public void Should_Throw_Exception_For_Invalid_Name()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			// Act & Assert
			Assert.Throws<DomainException>(() => Venue.Register(new string('A', 51), admin, address, location));
		}

		[Fact]
		public void Should_Update_Profile_With_Valid_Name()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);

			var newAddress = Address.Build("Tehran", "Tehran", "خیابان انقلاب پلاک 10 واحد 1", "1371683737");
			var newLocation = GeoLocation.Build(35.6895, 51.3895);
			// Act
			venue.UpdateProfile("Updated Venue", newAddress, newLocation);
			// Assert
			Assert.Equal("Updated Venue", venue.Name);
			Assert.Equal(newAddress, venue.Address);
			Assert.Equal(newLocation, venue.Location);
			Assert.True(venue.Events.Any());
			Assert.True(venue.Events.Any(e => e is VenueProfileUpdatedEvent));
		}

		[Fact]
		public void Should_Throw_Exception_For_Invalid_Profile_Update_Name()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);
			var newAddress = Address.Build("Tehran", "Tehran", "خیابان انقلاب پلاک 10 واحد 1", "1371683737");
			var newLocation = GeoLocation.Build(35.6895, 51.3895);
			// Act & Assert
			Assert.Throws<DomainException>(() => venue.UpdateProfile(new string('B', 51), newAddress, newLocation));
		}

		[Fact]
		public void Should_Relocate_Raise_Event()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);
			var newAddress = Address.Build("Tehran", "Tehran", "خیابان انقلاب پلاک 10 واحد 1", "1371683737");
			var newLocation = GeoLocation.Build(35.6895, 51.3895);
			// Act
			venue.Relocate(newLocation, newAddress, "Relocating to a new address");
			// Assert
			Assert.Equal(newAddress, venue.Address);
			Assert.Equal(newLocation, venue.Location);
			Assert.True(venue.Events.Any());
			Assert.True(venue.Events.Any(e => e is VenueRelocatedEvent));
		}

		[Fact]
		public void Should_Add_Facility_With_Valid_Title()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);
			var facility = Facility.Build("جکوزی", "در ابعاد کوچک و بزرگ با آب گرم و سرد");
			// Act
			venue.AddFacility(facility);
			// Assert
			Assert.Contains(facility, venue.Facilities);
			Assert.True(venue.Events.Any());
			Assert.True(venue.Events.Any(e => e is FaclilitiesChangedEvent));
		}

		[Fact]
		public void Should_Throw_Exception_For_Duplicate_Facility()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);
			var facility = Facility.Build("جکوزی", "در ابعاد کوچک و بزرگ با آب گرم و سرد");
			venue.AddFacility(facility);
			// Act & Assert
			Assert.Throws<DomainException>(() => venue.AddFacility(facility));
		}

		[Fact]
		public void Should_Remove_Facility()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);
			var facility = Facility.Build("جکوزی", "در ابعاد کوچک و بزرگ با آب گرم و سرد");
			venue.AddFacility(facility);
			// Act
			venue.RemoveFacility(facility);
			// Assert
			Assert.DoesNotContain(facility, venue.Facilities);
			Assert.True(venue.Events.Any());
			Assert.True(venue.Events.Any(e => e is FaclilitiesChangedEvent));
		}

		[Fact]
		public void Should_Throw_Exception_For_Nonexistent_Facility_Removal()
		{
			// Arrange
			var admin = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin, address, location);
			var facility = Facility.Build("جکوزی", "در ابعاد کوچک و بزرگ با آب گرم و سرد");
			// Act & Assert
			Assert.Throws<DomainException>(() => venue.RemoveFacility(facility));
		}

		[Fact]
		public void Should_Add_Admin()
		{
			// Arrange
			var admin1 = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin1, address, location);
			var admin2 = VenueAdmin.Build("user456", DateTimeOffset.UtcNow);
			// Act
			venue.AddAdmin(admin2);
			// Assert
			Assert.Contains(admin2, venue.Admins);
		}

		[Fact]
		public void Should_Throw_Exception_For_Duplicate_Admin()
		{
			// Arrange
			var admin1 = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin1, address, location);
			var admin2 = VenueAdmin.Build("user123", DateTimeOffset.UtcNow); // Same userId as admin1
																			 // Act & Assert
			Assert.Throws<DomainException>(() => venue.AddAdmin(admin2));
		}

		[Fact]
		public void Should_Remove_Admin()
		{
			// Arrange
			var admin1 = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin1, address, location);
			var admin2 = VenueAdmin.Build("user456", DateTimeOffset.UtcNow);
			venue.AddAdmin(admin2);

			// Act
			venue.RemoveAdmin(admin2);

			// Assert
			Assert.DoesNotContain(admin2, venue.Admins);
		}

		[Fact]
		public void Should_Throw_Exception_For_Nonexistent_Admin_Removal()
		{
			// Arrange
			var admin1 = VenueAdmin.Build("user123", DateTimeOffset.UtcNow);
			var address = Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");
			var location = GeoLocation.Build(35.6892, 51.3890);
			var venue = Venue.Register("My Venue", admin1, address, location);
			var admin2 = VenueAdmin.Build("user456", DateTimeOffset.UtcNow); // Not added to the venue
			// Act & Assert
			Assert.Throws<DomainException>(() => venue.RemoveAdmin(admin2));
		}
	}
}