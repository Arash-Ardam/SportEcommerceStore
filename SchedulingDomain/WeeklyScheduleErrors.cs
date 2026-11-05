using SharedKernel;

namespace SchedulingDomain
{
	public static class WeeklyScheduleErrors
	{
		public static DomainException InvalidVenueId() => DomainException.Build("WeeklyScheduleDomain.InvalidVenueId", "شناسه مکان ورزشی مورد نظر نامعبر است");
		public static DomainException InvalidHorizon() => DomainException.Build("WeeklyScheduleDomain.InvalidHorizon", "مقدار تعداد روز تولیدی مد نظر نامعتبر است");
		public static DomainException RangeOverloaded() => DomainException.Build("WeeklyScheduleDomain.RangeOverloaded", "بازه مد نظر با سانس دیگری تداخل دارد");
		public static DomainException InvalidCapacity() => DomainException.Build("WeeklyScheduleDomain.InvalidCapacity", "ظرفیت مد نظر نامعتبر است");
		public static DomainException InvalidRange() => DomainException.Build("WeeklyScheduleDomain.InvalidRange", "شروع بازه زمانی نباید از پایان آن بیشتر باشد");
		public static DomainException InvalidAgeRange() => DomainException.Build("WeeklyScheduleDomain.InvalidAgeRange", "محدوده سنی وارد شده نامعتبر است");
		public static DomainException InvalidAmount() => DomainException.Build("WeeklyScheduleDomain.InvalidAmount","مبلغ وارد شده نا معتبر است");
		public static DomainException InvalidSlotId() => DomainException.Build("WeeklyScheduleDomain.InvalidSlotId", "شناسه ردیف برنامه سانس نا معتبر است");
	}
}
