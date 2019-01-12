using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace OpenWeatherClient.Tests.Unit
{
    public class OpenCageGeoTests
    {
        [Fact]
        public void GetLatLongForCityAsync__CallsToOpenCage()
        {
            var handlerMock = SetupBackend(x => x.RequestUri.AbsoluteUri.StartsWith("https://api.opencagedata.com/geocode/v1/json?"));

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Foobar");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongForCityAsync__CallsWithGet()
        {
            var handlerMock = SetupBackend(x => x.Method == HttpMethod.Get);

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Foobar");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheCityToTheQueryString()
        {
            var city = "Des Moines";
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"q={WebUtility.UrlEncode(city)}"));
            
            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync(city);

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheStateToTheQueryString()
        {
            var city = "Des Moines";
            var state = "Ia";
            var encodedLocation = WebUtility.UrlEncode($"{city},{state}");
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"q={encodedLocation}", StringComparison.InvariantCultureIgnoreCase));

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async() => await geo.GetLatLongForCityAsync(city, state);

            act.Should().NotThrow();
        }

        private Mock<HttpMessageHandler> SetupBackend(Expression<Func<HttpRequestMessage, bool>> match, Mock<HttpMessageHandler> mock = null)
        {
            mock = mock ?? new Mock<HttpMessageHandler>();
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(match),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                })
                .Verifiable();
            return mock;
        }
    }
}