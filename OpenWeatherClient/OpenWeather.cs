using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using OpenWeatherClient.Abstractions;

namespace OpenWeatherClient
{
    public class OpenWeather : IWeather
    {
        private readonly HttpClient client;
        private readonly string apiKey;

        public OpenWeather(HttpClient client, string apiKey)
        {
            if (client == null)
                throw new ArgumentException(nameof(client));
            if (String.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException(nameof(apiKey));

            this.client = client;
            this.apiKey = apiKey;
        }

        public Task<decimal> CurrentTempAsync(Coord location)
        {
            throw new NotImplementedException();
        }
    }
}