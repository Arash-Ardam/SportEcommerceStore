namespace SchedulingDomain
{
	public record AdmissionPolicy
	{
		public Gender Gender { get; private set; }
		public AgeRange? AgeRange { get; private set; }

		public static AdmissionPolicy Set(Gender gender, AgeRange range)
		{
			return new AdmissionPolicy
			{
				AgeRange = range,
				Gender = gender
			};
		}
	}
	
}
