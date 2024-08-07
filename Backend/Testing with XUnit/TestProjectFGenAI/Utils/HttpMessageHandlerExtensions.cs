using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectFGenAI.Utils
{
    public static  class HttpMessageHandlerExtensions
    {
        public static Mock<HttpMessageHandler> SetupRequest(this Mock<HttpMessageHandler> handlerMock, HttpMethod method, string url)
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method && req.RequestUri.AbsoluteUri == url),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();
            return handlerMock;
        }

        public static Mock<HttpMessageHandler> ReturnsResponse(this Mock<HttpMessageHandler> handlerMock, string responseBody, string mediaType)
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseBody, Encoding.UTF8, mediaType)
                })
                .Verifiable();
            return handlerMock;
        }

        public static Mock<HttpMessageHandler> ReturnsResponse(this Mock<HttpMessageHandler> handlerMock, HttpStatusCode statusCode)
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(statusCode))
                .Verifiable();
            return handlerMock;
        }
    }
}
