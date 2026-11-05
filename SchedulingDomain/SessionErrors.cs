using SharedKernel;

namespace SchedulingDomain
{
	public static class SessionErrors
	{
		public static DomainException InvalidDate() => DomainException.Build("SessionDomain.InvalidDate", "تاریخ سانس انتخابی با روز ردیف برنامه هفتگی تطابق ندارد");
		public static DomainException InvalidVenueId() => DomainException.Build("SessionDomain.InvalidVenueId", "شناسه مکان ورزشی مورد نظر نامعبر است");
		public static DomainException InvalidPricingStatus() => DomainException.Build("SessionDomain.InvalidPricingStatus", "امکان تغییر قیمت دهی برای سانس با وضعیت غیر از برنامه ریزی شده وجود ندارد.");
		public static DomainException InvalidCancellationTime() => DomainException.Build("SessionDomain.InvalidCancellationTime", "لغو سانس برای سانس شروع شده امکان پذیر نمی باشد");
		public static DomainException DuplicateCancellation() => DomainException.Build("SessionDomain.DuplicateCancellation", "عملیات لغو درخواست شده تکراری است");
		public static DomainException InvalidCancellationReason() => DomainException.Build("SessionDomain.InvalidCancellationReason", "علت لغو سانس نباید خالی باشد");

	}
}
