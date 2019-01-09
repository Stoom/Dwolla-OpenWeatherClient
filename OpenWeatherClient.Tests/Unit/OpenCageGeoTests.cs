using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace OpenWeatherClient.Tests.Unit
{
    public class OpenCageGeoTests
    {
        [Fact]
        public async Task GetLatLongForCityAsync__CallsToOpenCage()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsoluteUri.StartsWith("https://api.opencagedata.com/geocode/v1/json?")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                })
                .Verifiable();

            var client = new HttpClient(handlerMock.Object);
            var geo = new OpenCageGeo(client);

            await geo.GetLatLongForCityAsync("Foobar");

            handlerMock.Protected()
                .Verify("SendAsync", Times.Exactly(1));
        }
    }
}