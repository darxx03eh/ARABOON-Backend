using Araboon.Core.Bases;
using Araboon.Core.Exceptions;
using Araboon.Core.Translations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Araboon.Core.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ErrorHandlerMiddleware(RequestDelegate next, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.next = next;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task Invoke(HttpContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                await next(context);

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    await WriteErrorResponse(context, HttpStatusCode.Forbidden,
                        stringLocalizer[SharedTranslationKeys.Forbidden], options);
                    return;
                }
                else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    await WriteErrorResponse(context, HttpStatusCode.Unauthorized,
                        stringLocalizer[SharedTranslationKeys.Unauthorized], options);
                    return;
                }
            }
            catch (Exception error)
            {
                await HandleException(context, error, options);
            }
        }

        private async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode,
            string message, JsonSerializerOptions options)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var responseModel = new ApiResponse()
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = message
            };

            var result = JsonSerializer.Serialize(responseModel, options);
            await context.Response.WriteAsync(result, Encoding.UTF8);
        }

        private async Task HandleException(HttpContext context, Exception error, JsonSerializerOptions options)
        {
            if (context.Response.HasStarted)
                return;

            var responseModel = new ApiResponse()
            {
                Succeeded = false,
                Message = error?.Message
            };

            switch (error)
            {
                case UnauthorizedAccessException e:
                    responseModel.Message = stringLocalizer[SharedTranslationKeys.UnauthorizedAccessException];
                    responseModel.StatusCode = HttpStatusCode.Unauthorized;
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case CustomValidationException e:
                    responseModel.Message = stringLocalizer[SharedTranslationKeys.ValidationFailed];
                    responseModel.StatusCode = HttpStatusCode.UnprocessableEntity;
                    context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    responseModel.Errors = e.Errors;
                    break;

                case KeyNotFoundException e:
                    responseModel.Message = stringLocalizer[SharedTranslationKeys.KeyNotFoundException];
                    responseModel.StatusCode = HttpStatusCode.NotFound;
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case DbUpdateException e:
                    responseModel.Message = e.Message;
                    responseModel.StatusCode = HttpStatusCode.BadRequest;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    responseModel.Message = error.Message;
                    responseModel.Message += error.InnerException != null ? $"\n{error.InnerException.Message}" : "";
                    responseModel.StatusCode = HttpStatusCode.InternalServerError;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json; charset=utf-8";
            var result = JsonSerializer.Serialize(responseModel, options);
            await context.Response.WriteAsync(result, Encoding.UTF8);
        }
    }
}