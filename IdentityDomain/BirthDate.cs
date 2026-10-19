namespace IdentityDomain
{
	public record BirthDate
	{
		public DateOnly Value { get; }
		private BirthDate(DateOnly value) => Value = value;

		public static BirthDate Create(DateOnly value, DateOnly today)
		{
			if (value > today || value < today.AddYears(-120))
				throw UserProfileErrors.InvalidBirthdate();
			return new BirthDate(value);
		}

		public int AgeAt(DateOnly date)
		{
			var age = date.Year - Value.Year;
			if (Value > date.AddYears(-age)) age--;
			return age;
		}
	}
}
