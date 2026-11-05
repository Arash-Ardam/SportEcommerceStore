using SchedulingDomain;
using SchedulingDomain.Events;
using SharedKernel;

namespace SchedulingDomainTests
{
	public class SessionTests
	{
		[Fact]
		public void Should_Create_Session_From_Valid_Slot()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DateTime.Now.DayOfWeek;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			var slot = ScheduleSlot.Create(day, timeRange, type, capacity, admissionPolicy, pricingPolicy);
			var date = DateOnly.FromDateTime(DateTime.Now);

			// Act
			var session = Session.FromSlot(venueId, slot, date);
			var sessionCreatedEvent = session.Events.OfType<SessionScheduledEvent>().FirstOrDefault();

			// Assert
			Assert.NotNull(session);
			Assert.NotNull(sessionCreatedEvent);
			Assert.Equal(venueId, session.VenueId);
			Assert.Equal(slot.Id, session.SourceSlotId);
			Assert.Equal(date, session.Date);
			Assert.Equal(slot.TimeRange, session.TimeRange);
			Assert.Equal(slot.Type, session.Type);
			Assert.Equal(slot.Capacity, session.InitialCapacity);
			Assert.Equal(date, session.Date);
			Assert.Equal(slot.AdmissionPolicy, session.AdmissionPolicy);
			Assert.Equal(slot.PricingPolicy, session.PricingPolicy);
			Assert.Equal(SessionStatus.Scheduled,session.Status);
		}

		[Fact]
		public void Should_Throw_Exception_On_Create_Session_With_Invalid_date()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DateTime.Now.DayOfWeek;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			var slot = ScheduleSlot.Create(day, timeRange, type, capacity, admissionPolicy, pricingPolicy);
			var date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => Session.FromSlot(venueId, slot, date));
			Assert.Equal("SessionDomain.InvalidDate", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_Ceate_Session_With_Invalid_VenueId()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DateTime.Now.DayOfWeek;
			var timeRange = TimeRange.Set(TimeOnly.Parse("10:00"), TimeOnly.Parse("12:00"));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			var slot = ScheduleSlot.Create(day, timeRange, type, capacity, admissionPolicy, pricingPolicy);
			var date = DateOnly.FromDateTime(DateTime.Now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => Session.FromSlot(Guid.Empty, slot, date));
			Assert.Equal("SessionDomain.InvalidVenueId", exception.Code);
		}


		[Fact]
		public void Should_Cancel_Session_With_Valid_Reason()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DateTime.Now.DayOfWeek;
			var timeRange = TimeRange.Set(TimeOnly.Parse(DateTime.Now.ToString("HH:mm")), TimeOnly.Parse(DateTime.Now.AddHours(2).ToString("HH:mm")));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			var slot = ScheduleSlot.Create(day, timeRange, type, capacity, admissionPolicy, pricingPolicy);
			var date = DateOnly.FromDateTime(DateTime.Now);
			var session = Session.FromSlot(venueId, slot, date);
			var reason = "Instructor unavailable";

			// Act
			session.Cancel(reason,DateTime.Now.AddMinutes(-10));
			var cancellationEvent = session.Events.OfType<SessionCancelledEvent>().FirstOrDefault();

			// Assert
			Assert.Equal(SessionStatus.Cancelled, session.Status);
			Assert.Equal(reason, session.CancellationReason);
			Assert.NotNull(cancellationEvent);
		}

		[Fact]
		public void Should_Throw_Exception_On_Cancel_Session_With_Invalid_Reason()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DateTime.Now.DayOfWeek;
			var timeRange = TimeRange.Set(TimeOnly.Parse(DateTime.Now.ToString("HH:mm")), TimeOnly.Parse(DateTime.Now.AddHours(2).ToString("HH:mm")));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			var slot = ScheduleSlot.Create(day, timeRange, type, capacity, admissionPolicy, pricingPolicy);
			var date = DateOnly.FromDateTime(DateTime.Now);
			var session = Session.FromSlot(venueId, slot, date);
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => session.Cancel(string.Empty,DateTime.Now.AddMinutes(-10)));
			Assert.Equal("SessionDomain.InvalidCancellationReason", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_Cancel_Session_With_Duplicate_Cancellation()
		{
			// Arrange
			var venueId = Guid.NewGuid();
			var horizon = BookingHorizon.Set(10);
			var schedule = WeeklySchedule.Create(venueId, horizon);
			var day = DateTime.Now.DayOfWeek;
			var timeRange = TimeRange.Set(TimeOnly.Parse(DateTime.Now.ToString("HH:mm")), TimeOnly.Parse(DateTime.Now.AddHours(2).ToString("HH:mm")));
			var ageRange = AgeRange.Set(18, 60);
			var type = SessionType.Regular;
			var gender = Gender.Male;
			var admissionPolicy = AdmissionPolicy.Set(gender, ageRange);
			var pricingPolicy = PricingPolicy.Set(100, 80);
			var capacity = 50;
			var slot = ScheduleSlot.Create(day, timeRange, type, capacity, admissionPolicy, pricingPolicy);
			var date = DateOnly.FromDateTime(DateTime.Now);
			var reason = "test-reason";
			var session = Session.FromSlot(venueId, slot, date);
			session.Cancel(reason, DateTime.Now.AddMinutes(-10));

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => session.Cancel(string.Empty, DateTime.Now.AddMinutes(-10)));
			Assert.Equal("SessionDomain.DuplicateCancellation", exception.Code);
		}

	}
}
