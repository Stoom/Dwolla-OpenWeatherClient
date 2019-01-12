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

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheCountryToTheQueryString()
        {
            var city = "Des Moines";
            var state = "Ia";
            var country = "US";
            var encodedLocation = WebUtility.UrlEncode($"{city},{state},{country}");
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"q={encodedLocation}", StringComparison.InvariantCultureIgnoreCase));

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async() => await geo.GetLatLongForCityAsync(city, state, country);

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheCountryWithoutStateToTheQueryString()
        {
            var city = "Des Moines";
            var country = "US";
            var encodedLocation = WebUtility.UrlEncode($"{city},{country}");
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"q={encodedLocation}", StringComparison.InvariantCultureIgnoreCase));

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            Func<Task> act = async() => await geo.GetLatLongForCityAsync(city, null, country);

            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetLatLongFromCityAsync__ReturnsTheCoordsForTheRequested()
        {
            var handlerMock = SetupBackend();

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            var coords = await geo.GetLatLongForCityAsync("Des Moines");

            coords.Lat.Should().BeApproximately(lat, 2d);
            coords.Lng.Should().BeApproximately(lng, 2d);
            coords.ToString().Should().BeEquivalentTo(location);
        }

        private Mock<HttpMessageHandler> SetupBackend(Expression<Func<HttpRequestMessage, bool>> match = null, Mock<HttpMessageHandler> mock = null)
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
                    Content = new StringContent(DefaultOkResponse)
                })
                .Verifiable();
            return mock;
        }

        private const double lat = 41.59d;
        private const double lng = -93.60d;
        private const string location = "Des Moines, IA 50309, United States of America";
        private readonly string DefaultOkResponse = $@"{{
            ""results"": [{{
                ""formatted"": ""{location}"",
                ""geometry"": {{
                    ""lat"": {lat},
                    ""lng"": {lng}
                }}
            }}],
            ""status"": {{
                ""code"": 200,
                ""message"": ""OK""
            }}
        }}
        ";

        private const string NoResultsFoundResponse = @"{
            ""results"": [],
            ""status"": {
                ""code"": 200,
                ""message"": ""OK""
            }
        }
        ";

        private const string BadKeyResponse = @"{
            ""results"": [],
            ""status"": {
                ""code"": 403,
                ""message"": ""invalid API key""
            }
        }
        ";
    }
}