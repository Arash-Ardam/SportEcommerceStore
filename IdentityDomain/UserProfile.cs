using IdentityDomain.Events;
using SharedKernel;
using System.Text.RegularExpressions;

namespace IdentityDomain
{
	public class UserProfile : AggregateRoot<string>
	{
		public string FullName { get; private set; } = string.Empty;
		public UserProfileGender Gender { get; private set; }
		public string NationalId { get; private set; } = string.Empty;
		public BirthDate BirthDate { get; private set; } = null!;


		public static UserProfile Register(string userId,string fullName, UserProfileGender gender, string nationalId, DateOnly birthdate, DateTime today)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw UserProfileErrors.InvalidUserId();

			ValidateFullName(fullName);
			ValidateNationalId(nationalId);

			var birthDate = BirthDate.Create(birthdate, DateOnly.FromDateTime(today));
			var user = new UserProfile
			{
				Id = userId.Trim(),
				FullName = fullName.Trim(),
				Gender = gender,
				NationalId = nationalId.Trim(),
				BirthDate = birthDate
			};

			user.Raise(new UserProfileRegisteredEvent(user.Id));

			return user;
		}

		

		public void ChangeName(string fullName)
		{
			ValidateFullName(fullName);

			if (fullName.Trim() == FullName)
				return;

			FullName = fullName.Trim();

			Raise(new UserProfileChangedEvent(Id));
		}

		public void CorrectIdentityInfo(UserProfileGender gender, string nationalId, DateOnly birthdate, DateTime today)
		{
			ValidateNationalId(nationalId);
			var birthDate = BirthDate.Create(birthdate, DateOnly.FromDateTime(today));

			if (nationalId.Trim() == NationalId && birthDate == BirthDate && gender == Gender)
				return;

			Gender = gender;
			NationalId = nationalId.Trim();
			BirthDate = birthDate;

			Raise(new UserProfileChangedEvent(Id));
		}

		private static void ValidateNationalId(string nationalId)
		{
			if (string.IsNullOrWhiteSpace(nationalId))
				throw UserProfileErrors.InvalidNationalId();
			if (nationalId.Trim().Length != 10)
				throw UserProfileErrors.InvalidNationalIdLength();
			if (!Regex.IsMatch(nationalId.Trim(), @"^\d+$"))
				throw UserProfileErrors.InvalidNationalIdInput();
		}
		private static void ValidateFullName(string fullName)
		{
			if (string.IsNullOrWhiteSpace(fullName))
				throw UserProfileErrors.InvalidFullName();
			if (fullName.Trim().Length > 50)
				throw UserProfileErrors.FullNameTooLong();
		}
	}
}
