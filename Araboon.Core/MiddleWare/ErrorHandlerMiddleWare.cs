using System.Net;
using System.Text.Json;
using Araboon.Core.Bases;
using Araboon.Core.Exceptions;
using Araboon.Core.Translations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.MiddleWare
{
    public class ErrorHandlerMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ErrorHandlerMiddleWare(RequestDelegate next, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.next = next;
            this.stringLocalizer = stringLocalizer;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task Invoke(HttpContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };
            context.RequestServices.GetRequiredService<IStringLocalizer<SharedTranslation>>();
            try
            {
                await next(context);
                if (context.Response.StatusCode.Equals(StatusCodes.Status403Forbidden))
                {
                    context.Response.ContentType = "application/json";
                    var responseModel = new ApiResponse()
                    {
                        Succeeded = false,
                        StatusCode = HttpStatusCode.Forbidden,
                        Message = stringLocalizer[SharedTranslationKeys.Forbidden]
                    };
                    var result = JsonSerializer.Serialize(responseModel, options);
                    await context.Response.WriteAsync(result);
                }
                else if (context.Response.StatusCode.Equals(StatusCodes.Status401Unauthorized))
                {
                    context.Response.ContentType = "application/json";
                    var responseModel = new ApiResponse()
                    {
                        Succeeded = false,
                        StatusCode = HttpStatusCode.Unauthorized,
                        Message = stringLocalizer[SharedTranslationKeys.Unauthorized]
                    };
                    var result = JsonSerializer.Serialize(responseModel, options);
                    await context.Response.WriteAsync(result);
                }
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
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
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case CustomValidationException e:
                        responseModel.Message = stringLocalizer[SharedTranslationKeys.ValidationFailed];
                        responseModel.StatusCode = HttpStatusCode.UnprocessableEntity;
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        responseModel.Errors = e.Errors;
                        break;
                    case KeyNotFoundException e:
                        responseModel.Message = stringLocalizer[SharedTranslationKeys.KeyNotFoundException];
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case DbUpdateException e:
                        responseModel.Message = e.Message;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case Exception e:
                        if (e.GetType().ToString().Equals("ApiException"))
                        {
                            responseModel.Message += e.Message;
                            responseModel.Message += e.InnerException is null ? "" : $"\n{e.InnerException.Message}";
                            responseModel.StatusCode = HttpStatusCode.BadRequest;
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        responseModel.Message = e.Message;
                        responseModel.Message += e.InnerException is null ? "" : $"\n{e.InnerException.Message}";
                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    default:
                        responseModel.Message = error.Message;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                }
                var result = JsonSerializer.Serialize(responseModel, options);
                await response.WriteAsync(result);
            }
        }
    }
}
