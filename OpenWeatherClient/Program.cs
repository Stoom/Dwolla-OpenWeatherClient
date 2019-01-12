using System;

namespace OpenWeatherClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var openWeatherKey = Environment.GetEnvironmentVariable("OPENWEATHER_KEY");
            var openCageKey = Environment.GetEnvironmentVariable("OPENCAGE_KEY");

            if (string.IsNullOrWhiteSpace(openCageKey))
                throw new ArgumentException("Missing OpenCage api key");
            if (string.IsNullOrWhiteSpace(openWeatherKey))
                throw new ArgumentException("Missing OpenWeather api key");

        }
    }
}
