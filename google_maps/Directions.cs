using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JarvOS
{
	public class Directions
	{
		public Location Origin { get; private set; }
		public Location Destination { get; private set; }

		/// <summary>
		/// Distance in meters from the origin to the destination
		/// </summary>
		public float Distance { get; private set; }
		public float DistanceMiles { get { return Distance * 0.000621371f; } }
		
		public TimeSpan Duration { get; private set; }

		public Directions(string origin_address, string destination_address)
		{
			Origin = new Location(origin_address);
			Destination = new Location(destination_address);

			var url = string.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}4&key={2}", Origin.Address, Destination.Address, DirectionsAPIKey.Instance.Key);

			JObject jsonObject = GetDataFromServer(url);

			JToken route = jsonObject["routes"][0];
			JToken overviewPolyline = route["overview_polyline"];
			string encodedPoints = (string)overviewPolyline["points"];

			JToken leg = route["legs"][0];

			Distance = (float)leg["distance"]["value"];
			Duration = TimeSpan.FromSeconds((double)leg["duration"]["value"]);

			JToken startLocation = leg["start_location"];
			Origin.SetLatitudeAndLongitude((float)startLocation["lat"], (float)startLocation["lng"]);

			JToken endLocation = leg["end_location"];
			Destination.SetLatitudeAndLongitude((float)endLocation["lat"], (float)endLocation["lng"]);
		}

		private JObject GetDataFromServer(string url)
		{
			WebResponse response = WebRequest.Create(url).GetResponse();

			var reader = new StreamReader(response.GetResponseStream());
			// json-formatted string from maps api
			string responseFromServer = reader.ReadToEnd();

			response.Close();

			return JObject.Parse(responseFromServer);
		}
	}
}