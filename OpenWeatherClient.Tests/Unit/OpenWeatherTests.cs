using System;
using System.Net.Http;
using FluentAssertions;
using Xunit;

namespace OpenWeatherClient.Tests.Unit
{
    public class OpenWeatherTests
    {
        private string apiKey = "foobar";

        [Fact]
        public void Ctor__RequiresApiKey()
        {
            Action act = () => new OpenCageGeo(new HttpClient(), null);

            act.Should().Throw<ArgumentException>().WithMessage("*apiKey*");
        }

        [Fact]
        public void Ctor__RequiresHttpClient()
        {
            Action act = () => new OpenCageGeo(null, "Fooboar");

            act.Should().Throw<ArgumentException>().WithMessage("*client*");
        }
        
    }
}