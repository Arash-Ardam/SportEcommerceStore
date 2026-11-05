using SchedulingDomain;
using SharedKernel;

namespace SchedulingDomainTests
{
	public class WeeklyScheduleTests
	{
		[Fact]
		public void Should_Create_With_Valid_Data()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);

			// Act
			var schedule = WeeklySchedule.Create(venueId, horizon);

			// Assert
			Assert.Equal(venueId, schedule.VenueId);
			Assert.Equal(horizon, schedule.BookingHorizon);
		}

		[Fact]
		public void Should_Throw_Exception_For_Invalid_VenueId()
		{
			// Arrange
			var invalidVenueId = Guid.Empty;
			var horizon = BookingHorizon.Set(10);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => WeeklySchedule.Create(invalidVenueId, horizon));
			Assert.Equal("WeeklyScheduleDomain.InvalidVenueId", exception.Code);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-15)]
		public void Should_Throw_Exception_For_Invalid_Horizon(int horizonValue)
		{
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => BookingHorizon.Set(horizonValue));
			Assert.Equal("WeeklyScheduleDomain.InvalidHorizon", exception.Code);
		}

		[Fact]
		public void Should_Set_TimeRange_With_Valid_Data()
		{
			// Arrange
			var startTime = TimeOnly.Parse("10:00");
			var endTime = TimeOnly.Parse("12:00");
			// Act
			var timeRange = TimeRange.Set(startTime, endTime);
			// Assert
			Assert.Equal(startTime, timeRange.Start);
			Assert.Equal(endTime, timeRange.End);
		}

		[Theory]
		[InlineData("12:00", "10:00")]
		[InlineData("15:00", "15:00")]
		public void Should_Throw_Exception_On_Set_Invalid_TimeRange(string start, string end)
		{
			// Arrange
			var startTime = TimeOnly.Parse(start);
			var endTime = TimeOnly.Parse(end);
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => TimeRange.Set(startTime, endTime));
			Assert.Equal("WeeklyScheduleDomain.InvalidRange", exception.Code);
		}

		[Fact]
		public void Should_Set_AgeRange_With_Valid_Data()
		{
			// Arrange
			var minAge = 18;
			var maxAge = 60;
			// Act
			var ageRange = AgeRange.Set(minAge, maxAge);
			// Assert
			Assert.Equal(minAge, ageRange.Min);
			Assert.Equal(maxAge, ageRange.Max);
		}

		[Theory]
		[InlineData(60, 18)]
		[InlineData(0, 18)]
		[InlineData(18, 0)]
		[InlineData(10, 10)]
		[InlineData(-10, 10)]
		[InlineData(10, -10)]
		public void Should_Throw_Exception_On_Set_Invalid_AgeRange(int minAge, int maxAge)
		{
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => AgeRange.Set(minAge, maxAge));
			Assert.Equal("WeeklyScheduleDomain.InvalidAgeRange", exception.Code);
		}

		[Fact]
		public void Should_Set_AdmissionPolicy_With_Valid_Data()
		{
			// Arrange
			var ageRange = AgeRange.Set(18, 60);
			var gender = Gender.Male;
			// Act
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			// Assert
			Assert.Equal(gender, admissionPolicy.Gender);
			Assert.Equal(ageRange, admissionPolicy.AgeRange);
		}

		[Fact]
		public void Should_Set_PricingPolicy_With_Valid_Data()
		{
			// Arrange
			var basePrice = 100;
			var lastMinutePrice = 80;
			// Act
			var pricingPolicy = PricingPolicy.Set(basePrice, lastMinutePrice);
			// Assert
			Assert.Equal(basePrice, pricingPolicy.BasePrice);
			Assert.Equal(lastMinutePrice, pricingPolicy.LastMinutePrice);
		}

		[Theory]
		[InlineData(0, 120)]
		[InlineData(120, 0)]
		[InlineData(0, 0)]
		[InlineData(-120, 120)]
		[InlineData(120, -120)]
		[InlineData(-120, -120)]
		public void Should_Throw_Exception_On_Set_Invalid_PricingPolicy(long basePrice, long lastMinutePrice)
		{
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => PricingPolicy.Set(basePrice, lastMinutePrice));
			Assert.Equal("WeeklyScheduleDomain.InvalidAmount", exception.Code);
		}

		[Fact]
		public void Should_Add_Slot_With_Valid_Data()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;

			// Act
			schedule.AddSlot(day, timeRange, type, admissionPolicy, pricingPolicy, capacity);

			// Assert
			Assert.Single(schedule.Slots);
			var slot = schedule.Slots.First();
			Assert.Equal(day, slot.Day);
			Assert.Equal(timeRange, slot.TimeRange);
			Assert.Equal(type, slot.Type);
			Assert.Equal(admissionPolicy, slot.AdmissionPolicy);
			Assert.Equal(pricingPolicy, slot.PricingPolicy);
			Assert.Equal(capacity, slot.Capacity);
		}

		[Theory]
		[InlineData("8:00", "11:00")]
		[InlineData("10:30", "11:30")]
		[InlineData("11:00", "13:00")]
		public void Should_Throw_Exception_On_Add_Slot_With_Overload_TimeRange(string start, string end)
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			schedule.AddSlot(day, timeRange, type, admissionPolicy, pricingPolicy, capacity);

			var overlappingTimeRange = TimeRange.Set(TimeOnly.Parse(start), TimeOnly.Parse(end));

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => schedule.AddSlot(day, overlappingTimeRange, type, admissionPolicy, pricingPolicy, capacity));
			Assert.Equal("WeeklyScheduleDomain.RangeOverloaded", exception.Code);
		}

		[Fact]
		public void Should_Update_Slot_With_Valid_Data()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			schedule.AddSlot(day, timeRange, type, admissionPolicy, pricingPolicy, capacity);
			// Act
			var slot = schedule.Slots.First();
			var newTimeRange = TimeRange.Set(TimeOnly.Parse("11:00"), TimeOnly.Parse("13:00"));
			schedule.UpdateSlot(slot.Id, day, newTimeRange, type, admissionPolicy, pricingPolicy, capacity);
			// Assert
			Assert.Single(schedule.Slots);
			Assert.Equal(newTimeRange, slot.TimeRange);
		}

		[Fact]
		public void Should_Throw_Exception_On_Update_Slot_With_Overload_TimeRange()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange1 = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var timeRange2 = TimeRange.Set(TimeOnly.Parse("12:30"), TimeOnly.Parse("14:30"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			schedule.AddSlot(day, timeRange1, type, admissionPolicy, pricingPolicy, capacity);
			schedule.AddSlot(day, timeRange2, type, admissionPolicy, pricingPolicy, capacity);
			var slotToUpdate = schedule.Slots.First();
			var overlappingTimeRange = TimeRange.Set(TimeOnly.Parse("11:00"), TimeOnly.Parse("13:00"));
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => schedule.UpdateSlot(slotToUpdate.Id, day, overlappingTimeRange, type, admissionPolicy, pricingPolicy, capacity));
			Assert.Equal("WeeklyScheduleDomain.RangeOverloaded", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_Update_Slot_With_Invalid_Id()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			schedule.AddSlot(day, timeRange, type, admissionPolicy, pricingPolicy, capacity);
			var invalidSlotId = Guid.NewGuid();
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => schedule.UpdateSlot(invalidSlotId, day, timeRange, type, admissionPolicy, pricingPolicy, capacity));
			Assert.Equal("WeeklyScheduleDomain.InvalidSlotId", exception.Code);
		}

		[Fact]
		public void Should_Remove_Slot_With_Valid_Id()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			schedule.AddSlot(day, timeRange, type, admissionPolicy, pricingPolicy, capacity);
			var slotToRemove = schedule.Slots.First();
			// Act
			schedule.RemoveSlot(slotToRemove.Id);
			// Assert
			Assert.Empty(schedule.Slots);
		}

		[Theory]
		[MemberData(nameof(GuidTestData))]
		public void Should_Throws_Exception_On_Remove_Slot_With_Invalid_Id(Guid slotId)
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DayOfWeek.Monday;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			schedule.AddSlot(day, timeRange, type, admissionPolicy, pricingPolicy, capacity);
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => schedule.RemoveSlot(slotId));
			Assert.Equal("WeeklyScheduleDomain.InvalidSlotId", exception.Code);
		}

		[Fact]
		public void Should_Change_Booking_Horizon_With_Valid_Data()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var initialHorizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, initialHorizon);
			// Act
			schedule.ChangeBookingHorizon(20);
			// Assert
			Assert.Equal(20, schedule.BookingHorizon.Value);
		}


		public static TheoryData<Guid> GuidTestData => new()
		{
			Guid.Empty,
			Guid.Parse("550e8400-e29b-41d4-a716-446655440000")
		};
	}


}
