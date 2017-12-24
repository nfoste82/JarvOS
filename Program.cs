using System;
using System.Collections.Generic;
using System.Linq;

namespace JarvOS
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Any())
            {
                Console.WriteLine("Args:");
            }

            foreach (string arg in args)
            {
                Console.WriteLine(arg);
            }

            string origin = "1313 Disneyland Dr, Anaheim, CA 92802";
            string destination = "1600 Pennsylvania Ave NW, Washington, DC 20500";
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-o")
                {
                    if ((i + 1) < args.Length)
                    {
                        string originStart = args[i + 1];
                        if (originStart[0] != '"')
                        {
                            throw new System.ApplicationException("Origin address must be wrapped in quotations.");
                        }
                        i += 2;

                        while (i < args.Length)
                        {
                            originStart += args[i];
                            if (originStart.EndsWith('"'))
                            {
                                break;
                            }
                        }

                    }
                }
                else if (args[i] == "-d")
                {
                    if ((i + 1) < args.Length)
                    {
                        string destinationStart = args[i + 1];
                        if (destinationStart[0] != '"')
                        {
                            throw new System.ApplicationException("Origin address must be wrapped in quotations.");
                        }
                        i += 2;

                        while (i < args.Length)
                        {
                            destinationStart += args[i];
                            if (destinationStart.EndsWith('"'))
                            {
                                break;
                            }
                        }
                    }
                }
            }

			Directions dir = new Directions(origin, destination);

            Console.WriteLine("Origin lat: " + dir.Origin.Latitude + ", long: " + dir.Origin.Longitude);

            var originWeather = new Weather(dir.Origin);

            WeatherData? nullableData = originWeather.GetWeatherEstimateByDateTime(DateTime.UtcNow);

            if (nullableData.HasValue)
            {
                WeatherData data = nullableData.Value;

                Console.WriteLine("Weather at " + originWeather.Location.Address + ", at " + data.TimeStamp.ToLocalTime() + ", temp: " + data.TemperatureFahrenheit + "°F, humidity: " + (data.Humidity * 100.0f).ToString("0") + ", wind: " + data.WindSpeedMPH + " mph");
            }

            var destinationWeather = new Weather(dir.Destination);

            nullableData = destinationWeather.GetWeatherEstimateByDateTime(DateTime.UtcNow + dir.Duration);

            if (nullableData.HasValue)
            {
                WeatherData data = nullableData.Value;

                Console.WriteLine("Weather at " + destinationWeather.Location.Address + ", at " + data.TimeStamp.ToLocalTime() + ", temp: " + data.TemperatureFahrenheit + "°F, humidity: " + (data.Humidity * 100.0f).ToString("0") + ", wind: " + data.WindSpeedMPH + " mph");
            }
		}
    }
}
