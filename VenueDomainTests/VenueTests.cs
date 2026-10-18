using SharedKernel;
using VenueDomain;
using VenueDomain.Events;

namespace VenueDomainTests
{
	public class VenueTests
	{
		private static readonly DateTimeOffset Now = new(2026, 10, 18, 10, 0, 0, TimeSpan.FromHours(3.5));

		private static Address NewAddress() =>
			Address.Build("Tehran", "Tehran", "بلوار آزادی خیابان جلال خوشی پلاک 20 واحد 2", "1371683736");

		private static GeoLocation NewLocation() => GeoLocation.Build(35.6892, 51.3890);

		private static Venue NewVenue() => Venue.Register("My Venue", "user123", NewAddress(), NewLocation(), Now);

		private static string ErrorCode(Action act) => Assert.Throws<DomainException>(act).Code;

		// ---------- Register ----------

		[Fact]
		public void Register_Creates_Active_Venue_With_First_Admin_And_Raises_Event()
		{
			var venue = NewVenue();

			Assert.Equal("My Venue", venue.Name);
			Assert.Equal(VenueStatus.Active, venue.Status);
			Assert.Equal("user123", Assert.Single(venue.Admins).UserId);
			Assert.IsType<VenueCreatedEvent>(Assert.Single(venue.Events));
		}

		[Theory]
		[InlineData("")]
		[InlineData("   ")]
		public void Register_With_Empty_Name_Throws(string name) =>
			Assert.Equal("NULL_VENUE_NAME",
				ErrorCode(() => Venue.Register(name, "user123", NewAddress(), NewLocation(), Now)));

		[Fact]
		public void Register_With_Null_Name_Throws() =>
			Assert.Equal("NULL_VENUE_NAME",
				ErrorCode(() => Venue.Register(null!, "user123", NewAddress(), NewLocation(), Now)));

		[Fact]
		public void Register_With_Too_Long_Name_Throws() =>
			Assert.Equal("INVALID_VENUE_NAME",
				ErrorCode(() => Venue.Register(new string('A', Venue.NameMaxLength + 1), "user123", NewAddress(), NewLocation(), Now)));

		// ---------- Profile / Relocate ----------

		[Fact]
		public void UpdateProfile_Changes_Name_And_Raises_Event()
		{
			var venue = NewVenue();
			venue.ClearEvents();

			venue.UpdateProfile("Updated Venue");

			Assert.Equal("Updated Venue", venue.Name);
			Assert.IsType<VenueProfileUpdatedEvent>(Assert.Single(venue.Events));
		}

		[Fact]
		public void Relocate_With_Same_Values_Does_Not_Raise_Event()
		{
			var venue = NewVenue();
			venue.ClearEvents();

			venue.Relocate(NewLocation(), NewAddress()); // نمونه‌ی جدید، مقادیر یکسان

			Assert.Empty(venue.Events);
		}

		[Fact]
		public void Relocate_Raises_Event_With_New_Location()
		{
			var venue = NewVenue();
			venue.ClearEvents();
			var newLocation = GeoLocation.Build(35.70, 51.40);

			venue.Relocate(newLocation, NewAddress());

			var e = Assert.IsType<VenueRelocatedEvent>(Assert.Single(venue.Events));
			Assert.Equal(newLocation, e.Location);
		}

		// ---------- Facilities ----------

		[Fact]
		public void AddFacility_Duplicate_Title_Throws()
		{
			var venue = NewVenue();
			venue.AddFacility(Facility.Build("جکوزی", "آب گرم"));

			Assert.Equal("DUPLICATE_FACILITY",
				ErrorCode(() => venue.AddFacility(Facility.Build("جکوزی", "توضیح دیگر"))));
		}

		[Fact]
		public void RemoveFacility_By_Title_Removes_It()
		{
			var venue = NewVenue();
			venue.AddFacility(Facility.Build("جکوزی", "آب گرم"));

			venue.RemoveFacility("جکوزی");

			Assert.Empty(venue.Facilities);
			Assert.IsType<FacilitiesChangedEvent>(venue.Events.Last());
		}

		[Fact]
		public void RemoveFacility_Unknown_Throws() =>
			Assert.Equal("FACILITY_NOT_FOUND", ErrorCode(() => NewVenue().RemoveFacility("سونا")));

		// ---------- Admins ----------

		[Fact]
		public void AddAdmin_Duplicate_Throws() =>
			Assert.Equal("DUPLICATE_ADMIN", ErrorCode(() => NewVenue().AddAdmin("user123", Now)));

		[Fact]
		public void RemoveAdmin_By_UserId_Removes_It()
		{
			var venue = NewVenue();
			venue.AddAdmin("user456", Now);

			venue.RemoveAdmin("user456");

			Assert.DoesNotContain(venue.Admins, a => a.UserId == "user456");
		}

		[Fact]
		public void RemoveAdmin_Last_Admin_Throws() =>
			Assert.Equal("ADMIN_LIMIT", ErrorCode(() => NewVenue().RemoveAdmin("user123")));

		[Fact]
		public void RemoveAdmin_Unknown_Throws() =>
			Assert.Equal("ADMIN_NOT_FOUND", ErrorCode(() => NewVenue().RemoveAdmin("nobody")));

		// ---------- Status ----------

		[Fact]
		public void Suspend_Twice_Throws()
		{
			var venue = NewVenue();
			venue.Suspend();

			Assert.Equal("INVALID_VENUE_STATUS_TRANSITION", ErrorCode(() => venue.Suspend()));
		}

		[Fact]
		public void Activate_When_Already_Active_Throws() =>
			Assert.Equal("INVALID_VENUE_STATUS_TRANSITION", ErrorCode(() => NewVenue().Activate()));

		// ---------- Value Objects ----------

		[Fact]
		public void Address_With_Same_Values_Are_Equal() =>
			Assert.Equal(NewAddress(), NewAddress());

		[Theory]
		[InlineData("12345")]
		[InlineData("12345abcde")]
		public void Address_Invalid_PostalCode_Throws(string postalCode) =>
			Assert.Equal("INVALID_POSTAL_CODE",
				ErrorCode(() => Address.Build("Tehran", "Tehran", "جزئیات", postalCode)));

		[Theory]
		[InlineData(91)]
		[InlineData(double.NaN)]
		public void GeoLocation_Invalid_Latitude_Throws(double latitude) =>
			Assert.Equal("INVALID_LATITUDE", ErrorCode(() => GeoLocation.Build(latitude, 51)));
	}
}
