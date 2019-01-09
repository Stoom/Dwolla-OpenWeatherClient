namespace OpenWeatherClient.Abstractions
{
    public sealed class Coord
    {
        public double Lat { get; }
        public double Long { get; }

        public Coord(double latitude, double longitude) {
            Lat = latitude;
            Long = longitude;
        }
    }
}