using System.Threading.Tasks;

namespace OpenWeatherClient.Abstractions
{
    public interface IGeo
    {
         Task<Coord> GetLatLongForCityAsync(string location);
    }
}