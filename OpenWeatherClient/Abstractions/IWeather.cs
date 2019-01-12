using System.Threading.Tasks;

namespace OpenWeatherClient.Abstractions
{
    public interface IWeather
    {
         Task<decimal> CurrentTempAsync(Coord location);
    }
}