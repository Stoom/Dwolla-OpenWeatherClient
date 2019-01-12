namespace OpenWeatherClient
{
    public class OpenWeatherRes
    {
        public class MainRes
        {
            public double Temp { get; set; }
        }

        public MainRes Main { get; set; }
        public int Cod { get; set; }
        public string Message { get; set; }
    }
}