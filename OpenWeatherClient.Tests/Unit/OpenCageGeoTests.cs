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
    public class OpenCageGeoTests
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
        public void GetLatLongForCityAsync__CallsToOpenCage()
        {
            var handlerMock = SetupBackend(x => x.RequestUri.AbsoluteUri.StartsWith("https://api.opencagedata.com/geocode/v1/json?"));

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Foobar");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongForCityAsync__CallsWithGet()
        {
            var handlerMock = SetupBackend(x => x.Method == HttpMethod.Get);

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Foobar");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheApiKey()
        {
            var apiKey = "Foobar";
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"key={apiKey}"));
            
            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

            Func<Task> act = async () => await geo.GetLatLongForCityAsync("Des Moines");

            act.Should().NotThrow();
        }

        [Fact]
        public void GetLatLongFromCityAsync__AddsTheCityToTheQueryString()
        {
            var city = "Des Moines";
            var handlerMock = SetupBackend(x => x.RequestUri.Query.Contains($"q={WebUtility.UrlEncode(city)}"));
            
            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

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
            var geo = new OpenCageGeo(client, apiKey);

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
            var geo = new OpenCageGeo(client, apiKey);

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
            var geo = new OpenCageGeo(client, apiKey);

            Func<Task> act = async() => await geo.GetLatLongForCityAsync(city, null, country);

            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetLatLongFromCityAsync__ReturnsTheCoordsForTheRequested()
        {
            var handlerMock = SetupBackend();

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

            var coords = await geo.GetLatLongForCityAsync("Des Moines");

            coords.Lat.Should().BeApproximately(lat, 2d);
            coords.Lng.Should().BeApproximately(lng, 2d);
            coords.ToString().Should().BeEquivalentTo(location);
        }

        [Fact]
        public void GetLatLongFromCityAsync__ThrowsExceptionWhenBadRespons()
        {
            var handlerMock = SetupBackend(response: BadKeyResponse);

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

            Func<Task> act = async() => await geo.GetLatLongForCityAsync("Des Moines");

            act.Should().Throw<ApiException>().WithMessage($"*{BadKeyMessage}");
        }

        [Fact]
        public void GetLatLongFromCityAsync__ThrowsExceptionWhenNoLocationFound()
        {
            var handlerMock = SetupBackend(response: NoResultsFoundResponse);

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client, apiKey);

            Func<Task> act = async() => await geo.GetLatLongForCityAsync("Des Moines");

            act.Should().Throw<ApiException>().WithMessage($"*Location not found");
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
                    Content = new StringContent(response ?? DefaultOkResponse)
                })
                .Verifiable();
            return mock;
        }

        private const double lat = 41.59d;
        private const double lng = -93.60d;
        private const string location = "Des Moines, IA 50309, United States of America";
        private const string BadKeyMessage = "";
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

        private readonly string BadKeyResponse = $@"{{
            ""results"": [],
            ""status"": {{
                ""code"": 403,
                ""message"": ""{BadKeyMessage}""
            }}
        }}
        ";
    }
}