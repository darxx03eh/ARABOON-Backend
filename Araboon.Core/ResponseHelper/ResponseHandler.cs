using Araboon.Core.Bases;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Araboon.Core.ResponseHelper
{
    public static class ResponseHandler
    {
        private static readonly JsonSerializerOptions JSONOPTIONS = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        public static async Task WriteJsonResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {

            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new ApiResponse
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = message
            };

            var json = JsonSerializer.Serialize(response, JSONOPTIONS);
            await context.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}
