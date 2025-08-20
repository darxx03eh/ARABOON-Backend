using System.Net;

namespace Araboon.Core.Bases
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public Object Meta { get; set; }
        public Boolean Succeeded { get; set; }
        public String Message { get; set; }
        public Object Errors { get; set; }
        public Object Data { get; set; }
        public ApiResponse() { }
        public ApiResponse(Object data, String message = null)
            => (Succeeded, Message, Data) = (true, message, data);
        public ApiResponse(String message)
            => (Succeeded, Message) = (false, message);
        public ApiResponse(String message, Boolean succeeded)
            => (Message, Succeeded) = (message, succeeded);
    }
}
