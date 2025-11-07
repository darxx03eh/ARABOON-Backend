using Araboon.Core.Bases;
using Araboon.Core.Features.Users.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Queries.Handlers
{
    public class UserQueryHandler : ApiResponseHandler
        , IRequestHandler<GetUserProfileQuery, ApiResponse>
        , IRequestHandler<GetUsersForDashboardQuery, ApiResponse>
    {
        private readonly IUserService userService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UserQueryHandler(IUserService userService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.userService = userService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var (result, profile) = await userService.GetUserProfileAsync(request.UserName);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "UserFound" => Success(profile, message: stringLocalizer[SharedTranslationKeys.UserFound]),
                "ThereWasAProblemLoadingTheProfile" => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemLoadingTheProfile]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemLoadingTheProfile])
            };
        }

        public async Task<ApiResponse> Handle(GetUsersForDashboardQuery request, CancellationToken cancellationToken)
        {
            var (result, users, meta) = await userService.GetUsersForDashboardAsync(
                request.PageNumber, request.PageSize, request.Search
            );
            return result switch
            {
                "UsersNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UsersNotFound]),
                "UsersFound" => Success(users, meta, stringLocalizer[SharedTranslationKeys.UsersFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.UsersNotFound])
            };
        }
    }
}
