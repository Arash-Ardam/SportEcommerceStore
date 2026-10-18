namespace VenueDomain
{
    public record Address
    {
        public string Province { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string Details { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;

        private Address() { }

        public static Address Build(string province, string city, string details, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(province) ||
                string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(details))
                throw VenueErrors.InvalidAddress();

            // کد پستی ایران: دقیقاً ۱۰ رقم
            if (string.IsNullOrWhiteSpace(postalCode) ||
                postalCode.Trim().Length != 10 ||
                !postalCode.Trim().All(char.IsAsciiDigit))
                throw VenueErrors.InvalidPostalCode();

            return new Address
            {
                Province = province.Trim(),
                City = city.Trim(),
                Details = details.Trim(),
                PostalCode = postalCode.Trim()
            };
        }
    }
}
