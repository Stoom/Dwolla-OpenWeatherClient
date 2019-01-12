using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using OpenWeatherClient.Abstractions;
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

        [Fact]
        public void CurrentTempAsync__CallsOpenWeather()
        {
            var handlerMock = SetupBackend(x => x.RequestUri.AbsoluteUri.StartsWith("http://api.openweathermap.org/data/2.5/weather"));

            var client = new HttpClient(handlerMock.Object);
            var weather = new OpenWeather(client, apiKey);

            Func<Task> act = async () => await weather.CurrentTempAsync(new Coord(0d, 0d));

            act.Should().NotThrow();
        }

        [Fact]
        public void CurrentTempAsync__CallsOpenWithGet()
        {
            var handlerMock = SetupBackend(x => x.Method == HttpMethod.Get);

            var client = new HttpClient(handlerMock.Object);
            var weather = new OpenWeather(client, apiKey);

            Func<Task> act = async () => await weather.CurrentTempAsync(new Coord(0d, 0d));

            act.Should().NotThrow();
        }

        [Fact]
        public void CurrentTempAsync__AddsTheApiKey()
        {
            var apiKey = "Foobar";
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"key={apiKey}"));
            
            var client = new HttpClient(handlerMock.Object);
            var weather = new OpenWeather(client, apiKey);

            Func<Task> act = async () => await weather.CurrentTempAsync(new Coord(0d, 0d));

            act.Should().NotThrow();
        }

        [Fact]
        public void CurrentTempAsync__AddsTheLocation()
        {
            var coord = new Coord(41.65d, -93.71d);
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"lat={coord.Lat}") && x.RequestUri.Query.Contains($"lon={coord.Lng}"));
            
            var client = new HttpClient(handlerMock.Object);
            var weather = new OpenWeather(client, apiKey);

            Func<Task> act = async () => await weather.CurrentTempAsync(coord);

            act.Should().NotThrow();
        }

        [Fact]
        public void CurrentTempAsync__SpecifiesTheUnits()
        {
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"units=metric"));
            
            var client = new HttpClient(handlerMock.Object);
            var weather = new OpenWeather(client, apiKey);

            Func<Task> act = async () => await weather.CurrentTempAsync(new Coord(0d, 0d));

            act.Should().NotThrow();
        }
        
        private Mock<HttpMessageHandler> SetupBackend(
            Expression<Func<HttpRequestMessage, bool>> match = null,
            Mock<HttpMessageHandler> mock = null,
            string response = null)
        {
            mock = mock ?? new Mock<HttpMessageHandler>();
            var reqMatch = match != null
                ? ItExpr.Is<HttpRequestMessage>(match)
                : ItExpr.IsAny<HttpRequestMessage>();
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    reqMatch,
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response ?? "")
                })
                .Verifiable();
            return mock;
        }
    }
}