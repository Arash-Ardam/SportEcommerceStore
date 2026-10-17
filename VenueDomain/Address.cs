namespace VenueDomain
{
    public class Address
    {
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

		private Address()
		{
			
		}

        public static Address Build(string province, string city, string details, string postalCode)
			=> new Address
			{
				Province = province,
				City = city,
				Details = details,
				PostalCode = postalCode
			};
	}

}
