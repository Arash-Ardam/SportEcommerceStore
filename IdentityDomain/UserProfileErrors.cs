using SharedKernel;

namespace IdentityDomain
{
	public static class UserProfileErrors
	{
		public static DomainException InvalidUserId() => DomainException.Build("Identity.UserProfile.InvalidUserId", "UserId نمی تواند خالی باشد");
		public static DomainException InvalidFullName() => DomainException.Build("Identity.UserProfile.InvalidFullName", "نام و نام خانوادگی نمی تواند خالی باشد");
		public static DomainException FullNameTooLong() => DomainException.Build("Identity.UserProfile.FullNameToolong", "نام و نام خانوادگی نباید بیش از 50 کاراکتر باشد");
		public static DomainException InvalidNationalId() => DomainException.Build("Identity.UserProfile.InvalidNationalId", "شناسه ملی نمی تواند خالی باشد");
		public static DomainException InvalidNationalIdLength() => DomainException.Build("Identity.UserProfile.InvalidNationalIdLength", "طول شناسه ملی باید 10 کاراکتر باشد");
		public static DomainException InvalidNationalIdInput() => DomainException.Build("Identity.UserProfile.InvalidNationalIdInput", "شناسه ملی فقط شامل رقم باید باشد");
		public static DomainException InvalidBirthdate() => DomainException.Build("Identity.UserProfile.InvalidBirthdate", "تاریخ تولد نمی تواند در آینده باشد");
		public static DomainException InvalidDate() => DomainException.Build("Identity.UserProfile.InvalidDate", "تاریخ مشخص شده قبل از تاریخ تولد است");

	}
}
