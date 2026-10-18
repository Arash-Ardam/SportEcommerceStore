namespace VenueDomain
{
    public record Facility
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

		private Facility()
		{
			
		}

		public static Facility Build(string title, string description)
		{
			if(string.IsNullOrWhiteSpace(title))
			{
				throw VenueErrors.InvalidFacilityTitle();
			}

			return new Facility
			{
				Title = title,
				Description = description
			};
		}
	}
}
