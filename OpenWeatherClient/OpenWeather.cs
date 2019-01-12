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

        public async Task<double> CurrentTempAsync(Coord location)
        {
            var uri = new UriBuilder("http://api.openweathermap.org/data/2.5/weather");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["APPID"] = apiKey;
            query["lat"] = $"{location.Lat}";
            query["lon"] = $"{location.Lng}";
            query["units"] = "metric";
            uri.Query = query.ToString();

            var req = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
            var res = await client.SendAsync(req);

            var body = JsonConvert.DeserializeObject<OpenWeatherRes>(await res.Content.ReadAsStringAsync());

            if (body.Cod != (int)HttpStatusCode.OK)
                throw new ApiException(body.Message);

            return body.Main.Temp;
        }
    }
}