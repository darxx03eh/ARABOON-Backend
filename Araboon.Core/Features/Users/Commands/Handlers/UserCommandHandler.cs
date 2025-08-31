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
        , IRequestHandler<UploadCoverImageCommand, ApiResponse>
        , IRequestHandler<ChangeEmailCommand, ApiResponse>
        , IRequestHandler<ConfirmationChangeEmailCommand, ApiResponse>
        , IRequestHandler<ChangeBioCommand, ApiResponse>
        , IRequestHandler<ChangeNameCommand, ApiResponse>
        , IRequestHandler<DeleteProfileImageCommand, ApiResponse>
        , IRequestHandler<DeleteCoverImageCommand, ApiResponse>

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
                "FailedToDeleteOldImageFromCloudinary" => 
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteOldImageFromCloudinary]),
                "AnErrorOccurredWhileEditingImageData" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileEditingImageData]),
                "TheImageHasBeenChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.TheImageHasBeenChangedSuccessfully]),
                "AnErrorOccurredWhileProcessingYourProfileImageModificationRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingYourProfileImageModificationRequest]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingYourProfileImageModificationRequest])
            };
        }

        public async Task<ApiResponse> Handle(UploadCoverImageCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.UploadCoverImageAsync(request.OriginalImage, request.CroppedImage);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "FailedToDeleteOldOriginalImageFromCloudinary" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteOldOriginalImageFromCloudinary]),
                "FailedToDeleteOldCroppedImageFromCloudinary" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteOldCroppedImageFromCloudinary]),
                "AnErrorOccurredWhileEditingCoverImage" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileEditingCoverImage]),
                "TheCoverImageHasBeenChangedSuccessfully" => 
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheCoverImageHasBeenChangedSuccessfully]),
                "AnErrorOccurredWhileProcessingYourCoverImageModificationRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingYourCoverImageModificationRequest]),
                _ => InternalServerError()
            };
        }

        public async Task<ApiResponse> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.ChangeEmailAsync(request.Email);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileSendingTheChangeEmailPleaseTryAgain" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheChangeEmailPleaseTryAgain]),
                "ChangeEmailHasBeenSent" => Success(null, message: stringLocalizer[SharedTranslationKeys.ChangeEmailHasBeenSent]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheChangeEmailPleaseTryAgain])
            };
        }

        public async Task<ApiResponse> Handle(ConfirmationChangeEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.ChangeEmailConfirmationAsync(request.UserId, request.Email, request.Token);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "InvalidOrExpiredToken" => Unauthorized(stringLocalizer[SharedTranslationKeys.InvalidOrExpiredToken]),
                "EmailChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.EmailChangedSuccessfully]),
                "AnErrorOccurredDuringTheChangeEmailProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheChangeEmailProcess]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheChangeEmailProcess]),
            };
        }

        public async Task<ApiResponse> Handle(ChangeBioCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.ChangeBioAsync(request.Bio);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileChangingTheBio" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingTheBio]),
                "BioChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.BioChangedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingTheBio])
            };
        }

        public async Task<ApiResponse> Handle(ChangeNameCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.ChangeNameAsync(request.FirstName, request.LastName);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileChangingTheName" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingTheName]),
                "NameChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.NameChangedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingTheName])
            };
        }

        public async Task<ApiResponse> Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.DeleteProfileImage();
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "ThereIsNoImageToDelete" => BadRequest(stringLocalizer[SharedTranslationKeys.ThereIsNoImageToDelete]),
                "FailedToDeleteImageFromCloudinary" => 
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteImageFromCloudinary]),
                "ImageHasBeenSuccessfullyDeleted" => 
                Success(null, message: stringLocalizer[SharedTranslationKeys.ImageHasBeenSuccessfullyDeleted]),
                "AnErrorOccurredWhileSaving" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSaving]),
                "AnErrorOccurredWhileDeletingTheImage" => 
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheImage]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheImage])
            };
        }

        public async Task<ApiResponse> Handle(DeleteCoverImageCommand request, CancellationToken cancellationToken)
        {
            var result = await userService.DeleteCoverImage();
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "ThereIsNoImageToDelete" => BadRequest(stringLocalizer[SharedTranslationKeys.ThereIsNoImageToDelete]),
                "FailedToDeleteOriginalImageFromCloudinary" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteOriginalImageFromCloudinary]),
                "FailedToDeleteCroppedImageFromCloudinary" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteCroppedImageFromCloudinary]),
                "ImageHasBeenSuccessfullyDeleted" =>
                Success(null, message: stringLocalizer[SharedTranslationKeys.ImageHasBeenSuccessfullyDeleted]),
                "AnErrorOccurredWhileSaving" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSaving]),
                "AnErrorOccurredWhileDeletingTheImage" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheImage]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheImage])
            };
        }
    }
}
