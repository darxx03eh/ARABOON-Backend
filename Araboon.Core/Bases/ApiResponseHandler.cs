using Araboon.Core.Translations;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Bases
{
    public class ApiResponseHandler
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ApiResponseHandler(IStringLocalizer<SharedTranslation> stringLocalizer)
            => this.stringLocalizer = stringLocalizer;

        public ApiResponseHandler()
        {
        }

        public ApiResponse InternalServerError(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.InternalServerError] : message
            };
        public ApiResponse Conflict(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Conflict] : message
            };
        public ApiResponse NoContent(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.NoContent,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.NoContent] : message
            };
        public ApiResponse Deleted(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Deleted] : message
            };
        public ApiResponse Locked(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Locked,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.TheResourceThatIsBeingAccessedIsLocked] : message
            };
        public ApiResponse Success(Object entity, Object meta = null, string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = entity,
                Meta = meta,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Success] : message
            };

        public ApiResponse Accepted(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Accepted,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Accepted] : message
            };
        public ApiResponse Unauthorized(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Unauthorized] : message
            };
        public ApiResponse Forbidden(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Forbidden,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Forbidden] : message
            };
        public ApiResponse BadRequest(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.BadRequest] : message
            };
        public ApiResponse UnprocessableEntity(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.UnprocessableEntity] : message
            };
        public ApiResponse NotFound(string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.NotFound] : message
            };
        public ApiResponse Created(Object entity, Object meta = null, string message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Succeeded = true,
                Data = entity,
                Meta = meta,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Created] : message
            };
    }
}
