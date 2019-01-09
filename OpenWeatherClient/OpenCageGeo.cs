using System.Net.Http;
using System.Threading.Tasks;
using OpenWeatherClient.Abstractions;

namespace OpenWeatherClient
{
    public class OpenCageGeo : IGeo
    {
        public OpenCageGeo(HttpClient client)
        {

        }

        public async Task<Coord> GetLatLongForCityAsync(string city, string state = null, string country = null)
        {
            throw new System.NotImplementedException();
        }
    }
}