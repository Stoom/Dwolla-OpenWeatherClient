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
        private readonly string apiKey;

        public OpenCageGeo(HttpClient client, string apiKey)
        {
            if (client == null)
                throw new ArgumentException(nameof(client));
            if (String.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException(nameof(apiKey));

            this.client = client;
            this.apiKey = apiKey;
        }

        public async Task<Coord> GetLatLongForCityAsync(string location)
        {
            var uri = new UriBuilder("https://api.opencagedata.com/geocode/v1/json");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["q"] = location;
            query["key"] = apiKey;
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
    }
}