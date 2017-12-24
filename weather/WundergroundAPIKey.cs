using System.IO;

namespace JarvOS
{
	public class WundergroundAPIKey
	{
		public static WundergroundAPIKey Instance
		{
			get 
			{
				if (m_instance == null)
				{
					m_instance = new WundergroundAPIKey();
				}
				return m_instance;
			}
		}
		private static WundergroundAPIKey m_instance;
		
		/// <summary>
		/// Gets Wunderground API Key from file.
		/// Returns null if file cannot be found.
		/// <summary>
		public string Key
		{
			get
			{
				if (key == null)
				{
					var path = Directory.GetCurrentDirectory() + "/api_keys/wunderground.txt";

					key = File.ReadAllText(path);
				}

				return key;
			}
		}

		private WundergroundAPIKey() {}

		public string key = null;
	} 
}