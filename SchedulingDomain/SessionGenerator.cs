namespace SchedulingDomain
{
	public static class SessionGenerator
	{
		public static IReadOnlyList<Session> Generate(
			WeeklySchedule schedule,
			DateOnly today,
			IReadOnlySet<(DateOnly Date, Guid SlotId)> alreadyGenerated)
		{
			var result = new List<Session>();

			for (var i = 0; i < schedule.BookingHorizon.Value; i++)        // ۱. هر روز بازه
			{
				var date = today.AddDays(i);

				foreach (var slot in schedule.Slots.Where(s => s.Day == date.DayOfWeek))   // ۲. Slotهای این روز
				{
					if (alreadyGenerated.Contains((date, slot.Id)))      // ۳. قبلاً ساخته شده؟
						continue;

					result.Add(Session.FromSlot(schedule.VenueId, slot, date));
				}
			}
			return result;
		}
	}

}
