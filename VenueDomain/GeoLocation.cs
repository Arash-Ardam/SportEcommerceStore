namespace VenueDomain
{
    public class GeoLocation
    {
        public double Latitude { get; internal set; }
        public double Longitude { get; internal set; }

		private GeoLocation()
		{
			
		}

        public static GeoLocation Build(double latitude, double longitude)
            => new GeoLocation
            {
                Latitude = latitude,
                Longitude = longitude
            };
	}

}
