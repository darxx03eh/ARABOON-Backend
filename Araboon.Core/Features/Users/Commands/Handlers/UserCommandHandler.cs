using Araboon.Core.Bases;
using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Handlers
{
    public class UserCommandHandler : ApiResponseHandler
        , IRequestHandler<ChangePasswordCommand, ApiResponse>
        , IRequestHandler<ChangeUserNameCommand, ApiResponse>
        , IRequestHandler<UploadProfileImageCommand, ApiResponse>
    {
        private readonly IUserService userService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UserCommandHandler(IUserService userService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.userService = userService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.ChangePasswordAsync(request.CurrentPassword, request.NewPassword);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "TheCurrentPasswordIsWrong" => BadRequest(stringLocalizer[SharedTranslationKeys.TheCurrentPasswordIsWrong]),
                "AnErrorOccurredWhileDeletingTheOldPassword" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheOldPassword]),
                "AnErrorOccurredWhileAddingTheNewPassword" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheNewPassword]),
                "PasswordChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.PasswordChangedSuccessfully]),
                "AnErrorOccurredWhileChangingThePassword" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingThePassword]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingThePassword])
            };

        }

        public async Task<ApiResponse> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.ChangeUserNameAsync(request.UserName);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileChangingTheUsername" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingTheUsername]),
                "UsernameChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.UsernameChangedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingTheUsername])
            };
        }

        public async Task<ApiResponse> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.UploadProfileImageAsync(request.ProfileImage, request.CropData);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileEditingImageData" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileEditingImageData]),
                "TheImageHasBeenChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.TheImageHasBeenChangedSuccessfully]),
                "AnErrorOccurredWhileProcessingYourProfileImageModificationRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingYourProfileImageModificationRequest]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingYourProfileImageModificationRequest])
            };
        }
    }
}
