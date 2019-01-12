using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace OpenWeatherClient.Tests.Integration
{
    public class MainTests : IDisposable
    {
        string openWeatherKey = Environment.GetEnvironmentVariable("OPENWEATHER_KEY");
        string openCageKey = Environment.GetEnvironmentVariable("OPENCAGE_KEY");

        public MainTests()
        {
            if (string.IsNullOrWhiteSpace(openWeatherKey))
                openWeatherKey = "Foobar";
            if (string.IsNullOrWhiteSpace(openCageKey))
                openCageKey = "Fizzbuzz";
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("OPENCAGE_KEY", openCageKey);
            Environment.SetEnvironmentVariable("OPENWEATHER_KEY", openWeatherKey);
        }

        [Fact]
        public void Main__ThrowsWhenMissingOpenCageKey()
        {
            Environment.SetEnvironmentVariable("OPENCAGE_KEY", null);
            Func<Task> act = async () => await Program.Main(null);

            act.Should().Throw<ArgumentException>().WithMessage("*Missing OpenCage api key");
        }

        [Fact]
        public void Main__ThrowsWhenMissingOpenWeatherKey()
        {
            Environment.SetEnvironmentVariable("OPENWEATHER_KEY", null);
            Func<Task> act = async () => await Program.Main(null);

            act.Should().Throw<ArgumentException>().WithMessage("*Missing OpenWeather api key");
        }

        [Fact]
        public async Task Main__GetsWeatherForLocation()
        {
            using (var sw = new StringWriter())
            using (var sr = new  StringReader($"Des Moines, IA{Environment.NewLine}"))
            {
                Console.SetOut(sw);
                Console.SetIn(sr);

                await Program.Main(null);
                var output = sw.ToString();

                output.Should().Contain("degrees Celsius");
            }
        }
    }
}