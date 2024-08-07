using Xunit;
//using GenAIApp.Utils;
using System;
using GenAIApp.Utlis;
using GenAIApp.Models;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using TestProjectFGenAI.Utils;

namespace TestProjectFGenAI
{
    public class DocumentProcessorTest : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;

        private readonly HttpClient _httpClient;

        private  readonly AIClient aIClient;

        public DocumentProcessorTest( HttpClient httpClient)
        {
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _httpClient = new HttpClient(_handlerMock.Object);

            this.aIClient = new AIClient(httpClient);

        }

        public void Dispose()
        {
            // close down tests
        }

        [Fact]
        public async Task TestGetAnswer()
        {
            //Arrange: mock data

            string question = "what is my name";

            string documentText = "My name is khan. I love cola.";

            string expectedAnswer = "khan";

            var responseJson = new JObject
            {
                ["answer"] = expectedAnswer
            }.ToString();


            _handlerMock
           .SetupRequest(HttpMethod.Post, ApiUrls.questionAnsweringModelAPIUrl)
           .ReturnsResponse(responseJson, "application/json");
            //Act : you work here on data




            var response = await aIClient.GetAnswer(question, documentText);

            Console.WriteLine(response.Message.ToString());
            string actual = response.Message;
            //Assert test case
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Success);
            Assert.Equal(expectedAnswer, actual);
        }
    }
}