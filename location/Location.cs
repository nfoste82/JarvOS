namespace JarvOS
{
	public class Location
	{
		public string Address { get; private set; }
		public float Latitude { get; private set; }
		public float Longitude { get; private set; }

		public Location(string address)
		{
			Address = address;
		}

		public void SetLatitudeAndLongitude(float latitude, float longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}
	}
}