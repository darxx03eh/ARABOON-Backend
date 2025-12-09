using System.Net;

namespace Araboon.Core.Bases
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public Object Meta { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public Object Errors { get; set; }
        public Object Data { get; set; }
        public ApiResponse() { }
        public ApiResponse(Object data, string message = null)
            => (Succeeded, Message, Data) = (true, message, data);
        public ApiResponse(string message)
            => (Succeeded, Message) = (false, message);
        public ApiResponse(string message, bool succeeded)
            => (Message, Succeeded) = (message, succeeded);
    }
}
