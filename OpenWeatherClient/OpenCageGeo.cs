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
            var res = await client.SendAsync(req);

            var body = JsonConvert.DeserializeObject<OpenCageRes>(await res.Content.ReadAsStringAsync());

            if (body.Status.Code != (int)HttpStatusCode.OK)
                throw new ApiException(body.Status.Message);

            var result = body.Results.FirstOrDefault();
            if(result == null)
                throw new ApiException("Location not found");

            return new Coord(
                result.Geometry.Lat,
                result.Geometry.Lng,
                result.Formatted
            );
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