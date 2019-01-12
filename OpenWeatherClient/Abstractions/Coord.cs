namespace OpenWeatherClient.Abstractions
{
    public sealed class Coord
    {
        public double Lat { get; }
        public double Lng { get; }
        private string _dispalyName;

        public Coord(double latitude, double longitude, string displayName = null) {
            Lat = latitude;
            Lng = longitude;
            _dispalyName = displayName;
        }

        public override string ToString()
        {
            return _dispalyName ?? $"{Lat}, {Lng}";
        }
    }
}