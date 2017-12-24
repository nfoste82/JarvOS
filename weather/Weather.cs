using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace JarvOS
{
	public class Weather
	{
		public DateTime EarliestData { get; private set; }

		public Location Location { get; private set; }

		public Weather(Location location)
		{
			Location = location;

			var url = string.Format("http://api.wunderground.com/api/{0}/hourly/q/{1},{2}.json", WundergroundAPIKey.Instance.Key, location.Latitude, location.Longitude);

			JObject hourlyForecast = GetDataFromServer(url);

			List<JToken> hourForecasts = hourlyForecast["hourly_forecast"].ToList();

			foreach (var hourForecast in hourForecasts)
			{
				var data = WeatherData.CreateFromHourlyForecastData(hourForecast);

				int day = data.TimeStamp.Day;
				int hour = data.TimeStamp.Hour;

				Dictionary<int, WeatherData> hourData;
				if (!m_weatherDataSet.TryGetValue(day, out hourData))
				{
					hourData = new Dictionary<int, WeatherData>();
					m_weatherDataSet[day] = hourData;
				}

				hourData[hour] = data;
			}

			url = string.Format("http://api.wunderground.com/api/{0}/conditions/q/{1},{2}.json", WundergroundAPIKey.Instance.Key, location.Latitude, location.Longitude);
			JObject currentConditions = GetDataFromServer(url);

			var currentData = WeatherData.CreateFromCurrentConditionsData(currentConditions["current_observation"]);

			Dictionary<int, WeatherData> currentHourData;
			if (!m_weatherDataSet.TryGetValue(currentData.TimeStamp.Day, out currentHourData))
			{
				currentHourData = new Dictionary<int, WeatherData>();
				m_weatherDataSet[currentData.TimeStamp.Day] = currentHourData;
			}

			currentHourData[currentData.TimeStamp.Hour] = currentData;

			EarliestData = currentData.TimeStamp;
		}

		public WeatherData? GetWeatherEstimateByDateTime(DateTime time)
		{
			if (time < EarliestData)
			{
				throw new System.ApplicationException("Requested weather for DateTime prior to the earliest data.");
			}

			DateTime nextTime = time.AddHours(1);

			int day = time.Day;
			int hour = time.Hour;
			int nextHour = nextTime.Hour;
			int nextDay = nextTime.Day;
			
			Dictionary<int, WeatherData> hourData;
			if (!m_weatherDataSet.TryGetValue(day, out hourData))
			{
				// No data found for the given 'day'
				return null;
			}

			WeatherData startData;
			if (!hourData.TryGetValue(hour, out startData))
			{
				// No data found for the given 'hour'
				return null;
			}

			if (!m_weatherDataSet.TryGetValue(day, out hourData))
			{
				// No data found for 'nextDay'
				return startData;
			}

			WeatherData endData;
			if (!hourData.TryGetValue(hour, out endData))
			{
				// No data found for 'nextHour'
				return startData;
			}

			int startMinute = startData.TimeStamp.Minute;
			int minutesBetweenTimes = Math.Abs(60 - startMinute);

			// 18, 60		25
			int minsFromStart = time.Minute - startMinute;
			float percentageBetweenTimes = (float)minsFromStart / minutesBetweenTimes;

			float temp = MathUtils.Lerp(startData.Temperature, endData.Temperature, percentageBetweenTimes);
			float windSpeed = MathUtils.Lerp(startData.WindSpeed, endData.WindSpeed, percentageBetweenTimes);
			int windDir = (int)Math.Round(MathUtils.Lerp((float)startData.WindDirectionDegrees, (float)endData.WindDirectionDegrees, percentageBetweenTimes));
			float humidity = MathUtils.Lerp(startData.Humidity, endData.Humidity, percentageBetweenTimes);

			return new WeatherData(time, temp, windSpeed, windDir, humidity);
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

		// First key is day, second key is hour
		private Dictionary<int, Dictionary<int, WeatherData>> m_weatherDataSet = new Dictionary<int, Dictionary<int, WeatherData>>();
	}

	public struct WeatherData
	{
		public DateTime TimeStamp;

		/// <summary>
		/// Temperature in degrees celcius.
		/// <summary>
		public float Temperature;
		public float TemperatureFahrenheit { get { return 32.0f + Temperature * 1.8f; } }
		
		/// <summary>
		/// Humidity, as a percentage between 0.0 and 1.0.
		/// </summary>
		public float Humidity;

		/// <summary>
		/// Wind speed (in KPH)
		/// </summary>
		public float WindSpeed;

		public float WindSpeedMPH { get { return WindSpeed * 0.621371f; } }

		public int WindDirectionDegrees;

		public static WeatherData CreateFromHourlyForecastData(JToken data)
		{
			double secondsSinceUnixEpoch = (double)data["FCTTIME"]["epoch"];
			DateTime dt = TimeUtils.UnixTimeStampToDateTime(secondsSinceUnixEpoch);

			float temp = (float)data["temp"]["metric"];
			float windspeed = (float)data["wspd"]["metric"];
			int winddir = (int)data["wdir"]["degrees"];
			float humidity = ((int)data["humidity"]) * 0.01f;

			return new WeatherData(dt, temp, windspeed, winddir, humidity);
		}

		public static WeatherData CreateFromCurrentConditionsData(JToken data)
		{
			double secondsSinceUnixEpoch = (double)data["observation_epoch"];
			DateTime dt = TimeUtils.UnixTimeStampToDateTime(secondsSinceUnixEpoch);

			float temp = (float)data["temp_c"];

			float windspeed = (float)data["wind_kph"];
			int winddir = (int)data["wind_degrees"];

			string humidityStr = (string)data["relative_humidity"];
			humidityStr = humidityStr.TrimEnd('%');

			float humidity = float.Parse(humidityStr) * 0.01f;

			return new WeatherData(dt, temp, windspeed, winddir, humidity);
		}

		public WeatherData(DateTime timestamp, float temp, float windspeed, int winddir, float humidity)
		{
			Temperature = temp;
			TimeStamp = timestamp;
			WindSpeed = windspeed;
			WindDirectionDegrees = winddir;
			Humidity = humidity;
		}
	}
}