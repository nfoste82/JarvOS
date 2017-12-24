using System.IO;

namespace JarvOS
{
	public class DirectionsAPIKey
	{
		public static DirectionsAPIKey Instance
		{
			get 
			{
				if (m_instance == null)
				{
					m_instance = new DirectionsAPIKey();
				}
				return m_instance;
			}
		}
		private static DirectionsAPIKey m_instance;
		
		/// <summary>
		/// Gets Google Directions API Key from file.
		/// Returns null if file cannot be found.
		/// <summary>
		public string Key
		{
			get
			{
				if (key == null)
				{
					var path = Directory.GetCurrentDirectory() + "/api_keys/google_directions.txt";

					key = File.ReadAllText(path);
				}

				return key;
			}
		}

		private DirectionsAPIKey() {}

		public string key = null;
	} 
}