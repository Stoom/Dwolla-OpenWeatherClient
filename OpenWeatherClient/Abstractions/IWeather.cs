using System.Threading.Tasks;

namespace OpenWeatherClient.Abstractions
{
    public interface IWeather
    {
         Task<double> CurrentTempAsync(Coord location);
    }
}