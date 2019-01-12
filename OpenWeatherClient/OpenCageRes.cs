namespace OpenWeatherClient
{
    public class OpenCageRes
    {
        public class GeometryRes
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
        public class ResultRes
        {
            public GeometryRes Geometry { get; set; }
            public string Formatted { get; set; }
        }
        public class StatusRes
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }

        public StatusRes Status { get; set; }
        public ResultRes[] Results { get; set; }
    }
}