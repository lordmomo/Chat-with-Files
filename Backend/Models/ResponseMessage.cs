using System.Net;

namespace GenAIApp.Models
{
    public class ResponseMessage
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Success { get; set; }

        public string Message { get; set; }


    }
}
