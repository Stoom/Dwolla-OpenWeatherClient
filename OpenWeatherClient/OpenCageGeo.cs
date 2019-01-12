using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OpenWeatherClient.Abstractions;

namespace OpenWeatherClient
{
    public class OpenCageGeo : IGeo
    {
        private readonly HttpClient client;
        public OpenCageGeo(HttpClient client)
        {
            this.client = client;

        }

        public async Task<Coord> GetLatLongForCityAsync(string city, string state = null, string country = null)
        {
            var uri = new UriBuilder("https://api.opencagedata.com/geocode/v1/json");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["q"] = GenerateLocation(city, state, country);
            uri.Query = query.ToString();

            var req = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
            await client.SendAsync(req);

            return null;
        }

        private string GenerateLocation(string city, string state = null, string country = null)
        {
            var location = new StringBuilder(city);
            if (!String.IsNullOrWhiteSpace(state))
                location.Append($",{state}");
            if (!String.IsNullOrWhiteSpace(country))
                location.Append($",{country}");
            return location.ToString();
        }
    }
}