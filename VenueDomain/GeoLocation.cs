namespace VenueDomain
{
    public record GeoLocation
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

		private GeoLocation()
		{
			
		}

        public static GeoLocation Build(double latitude, double longitude)
        {
			if(latitude < -90 || latitude > 90 || double.IsNaN(latitude) || double.IsInfinity(latitude))
			{
				throw VenueErrors.InvalidLatitude();
			}

			if(longitude < -180 || longitude > 180 || double.IsNaN(longitude) || double.IsInfinity(longitude))
			{
				throw VenueErrors.InvalidLongitude();
			}

			return new GeoLocation
			{
				Latitude = latitude,
				Longitude = longitude
			};
		}
	}

}
