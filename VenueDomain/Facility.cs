namespace VenueDomain
{
    public class Facility
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

		private Facility()
		{
			
		}

		public static Facility Build(string title, string description)
			=> new Facility
			{
				Title = title,
				Description = description
			};
	}
}
