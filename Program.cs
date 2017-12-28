using System;
using System.Collections.Generic;
using System.Linq;

namespace JarvOS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment this and pass these args to EntryPoint if you want to be able to attach
            // and debug command line arguments.
            //string[] fakeArgs = new string[]{"-o", "1313 Disneyland Dr, Anaheim, CA 92802", "-d", "Walt Disney World Resort, Orlando, FL 32830"};
            
            EntryPoint(args);
        }

        static void EntryPoint(string[] args)
        {
            // Some defaults to use if nothing is passed in.
            string origin = "1313 Disneyland Dr, Anaheim, CA 92802";
            string destination = "Walt Disney World Resort, Orlando, FL 32830";

            bool help   = false;
            var p = new NDesk.Options.OptionSet () {
                { "h|?|help",   v => help = v != null },
                { "o|origin=",   v => origin = v },
                { "d|destination=",   v => destination = v }
            };
            p.Parse(args);

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
