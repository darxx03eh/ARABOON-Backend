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

        public ApiResponse InternalServerError(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.InternalServerError] : message
            };
        public ApiResponse Conflict(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Conflict,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Conflict] : message
            };
        public ApiResponse NoContent(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.NoContent,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.NoContent] : message
            };
        public ApiResponse Deleted(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Deleted] : message
            };
        public ApiResponse Locked(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Locked,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.TheResourceThatIsBeingAccessedIsLocked] : message
            };
        public ApiResponse Success(Object entity, Object meta = null, String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = entity,
                Meta = meta,
                Succeeded = true,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Success] : message
            };
        public ApiResponse Unauthorized(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Unauthorized] : message
            };
        public ApiResponse Forbidden(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.Forbidden,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.Forbidden] : message
            };
        public ApiResponse BadRequest(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.BadRequest] : message
            };
        public ApiResponse UnprocessableEntity(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.UnprocessableEntity] : message
            };
        public ApiResponse NotFound(String message = null)
            => new ApiResponse()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Succeeded = false,
                Message = message is null ? stringLocalizer[SharedTranslationKeys.NotFound] : message
            };
        public ApiResponse Created(Object entity, Object meta = null, String message = null)
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
