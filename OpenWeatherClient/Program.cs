using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using OpenWeatherClient.Abstractions;

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

            var client = new HttpClient();
            var geo = new OpenCageGeo(client, openCageKey);
            var weather = new OpenWeather(client, openWeatherKey);

            Console.Write("Where are you? ");
            var location = Console.ReadLine();

            try
            {
                var coord = await geo.GetLatLongForCityAsync(location);
                var temp = await weather.CurrentTempAsync(coord);

                Console.WriteLine($"{coord} wether:");
                Console.WriteLine($"{temp} degrees Celsius");
            }
            catch(ApiException ex) {
                Console.WriteLine("Could not retrieve weather:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(string.Empty);
        }
    }
}
