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
            var handlerMock = new Mock<HttpMessageHandler>();
            SetupBackend(handlerMock, x => x.RequestUri.AbsoluteUri.StartsWith("https://api.opencagedata.com/geocode/v1/json?"));

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Foobar");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongForCityAsync__CallsWithGet()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            SetupBackend(handlerMock, x => x.Method == HttpMethod.Get);

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Foobar");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheCityToTheQueryString()
        {
            var city = "Des Moines";
            var handlerMock = new Mock<HttpMessageHandler>();
            SetupBackend(handlerMock, x => x.RequestUri.Query.Contains($"q={WebUtility.UrlEncode(city)}"));
            
            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync(city);

            act.Should().NotThrow();
        }

        private void SetupBackend(Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> match)
        {
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
        }
    }
}