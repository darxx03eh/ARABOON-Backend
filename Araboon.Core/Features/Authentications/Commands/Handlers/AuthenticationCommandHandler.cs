using Araboon.Core.Bases;
using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Handlers
{
    public class AuthenticationCommandHandler : ApiResponseHandler
        , IRequestHandler<RegistrationUserCommand, ApiResponse>
        , IRequestHandler<ConfirmationEmailCommand, ApiResponse>
        , IRequestHandler<SignInCommand, ApiResponse>
        , IRequestHandler<SendConfirmationEmailCommand, ApiResponse>
        , IRequestHandler<GenerateRefreshTokenCommand, ApiResponse>
        , IRequestHandler<RevokeRefreshTokenCommand, ApiResponse>
        , IRequestHandler<SendForgetPasswordCommand, ApiResponse>
        , IRequestHandler<ForgetPasswordConfirmationCommand, ApiResponse>
        , IRequestHandler<ResetPasswordCommand, ApiResponse>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IAuthenticationService authenticationService;
        private readonly IMapper mapper;

        public AuthenticationCommandHandler(IStringLocalizer<SharedTranslation> stringLocalizer
                                          , IAuthenticationService authenticationService,
                                            IMapper mapper)
            : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authenticationService = authenticationService;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> Handle(RegistrationUserCommand request, CancellationToken cancellationToken)
        {
            var user = mapper.Map<AraboonUser>(request);
            var result = await authenticationService.RegistrationUserAsync(user, request.Password);
            return result switch
            {
                "ErrorDuringAccountCreationProcess" => InternalServerError(stringLocalizer[SharedTranslationKeys.ErrorDuringAccountCreationProcess]),
                "RoleNotExist" => NotFound(stringLocalizer[SharedTranslationKeys.RoleNotExist]),
                "ErrorWhileAddingRole" => InternalServerError(stringLocalizer[SharedTranslationKeys.ErrorWhileAddingRole]),
                "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain]),
                "AnErrorOccurredWhileAddingTheImage" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheImage]),
                "AnErrorOccurredDuringTheRegistrationProcess"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheRegistrationProcess]),
                _ => Created(null, message: stringLocalizer[SharedTranslationKeys.TheAccountHasBeenCreated])
            };
        }

        public async Task<ApiResponse> Handle(ConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.ConfirmationEmailAsync(request.Email, request.Token);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "InvalidOrExpiredToken" => Unauthorized(stringLocalizer[SharedTranslationKeys.InvalidOrExpiredToken]),
                "AnErrorOccurredDuringTheEmailConfirmationProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheEmailConfirmationProcess]),
                _ => Success(null, message: stringLocalizer[SharedTranslationKeys.EmailConfirmedSuccessfully])
            };
        }

        public async Task<ApiResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var (result, message) = await authenticationService.SignInAsync(request.UserName, request.Password);
            return message switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "EmailNotConfirmed" => Forbidden(stringLocalizer[SharedTranslationKeys.EmailNotConfirmed]),
                "PasswordOrUserNameWrnog" => Unauthorized(stringLocalizer[SharedTranslationKeys.PasswordOrUserNameWrnog]),
                "AnErrorOccurredWhileGeneratingTheToken" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileGeneratingTheToken]),
                "AnErrorOccurredDuringTheLoginProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheLoginProcess]),
                "YourAccountIsInactivePleaseCheckWithTechnicalSupport" =>
                Forbidden(stringLocalizer[SharedTranslationKeys.YourAccountIsInactivePleaseCheckWithTechnicalSupport]),
                "YourAccountWasLockedDueToSuspiciousActivityPleaseTryAgainLaterorContactSupport" =>
                Locked(stringLocalizer[SharedTranslationKeys.YourAccountWasLockedDueToSuspiciousActivityPleaseTryAgainLaterorContactSupport]),
                _ => Success(result, message: stringLocalizer[SharedTranslationKeys.DataVerifiedAndLogin])
            };
        }

        public async Task<ApiResponse> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.SendConfirmationEmailAsync(request.Email);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain]),
                "YourAccountHasAlreadyBeenConfirmed" => BadRequest(stringLocalizer[SharedTranslationKeys.YourAccountHasAlreadyBeenConfirmed]),
                _ => Success(null, message: stringLocalizer[SharedTranslationKeys.EmailConfirmationEmailHasBeenSent])
            };
        }

        public async Task<ApiResponse> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var (response, result) = await authenticationService.GenerateRefreshTokenAsync(request.AccessToken, request.RefreshToken);
            return result switch
            {
                "ErrorInTheEncryptionAlgorithmUsed" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.ErrorInTheEncryptionAlgorithmUsed]),
                "TokenIsStillValidCannotRefreshYet" => BadRequest(stringLocalizer[SharedTranslationKeys.TokenIsStillValidCannotRefreshYet]),
                "RefreshTokenIsNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.RefreshTokenIsNotFound]),
                "RefreshTokenHasExpire" => Unauthorized(stringLocalizer[SharedTranslationKeys.RefreshTokenHasExpire]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredDuringTheTokenGenerationProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheTokenGenerationProcess]),
                _ => Success(response, message: stringLocalizer[SharedTranslationKeys.AccessTokenRegenerated])
            };
        }

        public async Task<ApiResponse> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.RevokeRefreshTokenAsync(request.RefreshToken);
            return result switch
            {
                "InvalidRefreshToken" => BadRequest(stringLocalizer[SharedTranslationKeys.InvalidRefreshToken]),
                "AnErrorOccurredDuringTheRefreshTokenCancellationProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheRefreshTokenCancellationProcess]),
                _ => Success(null, message: stringLocalizer[SharedTranslationKeys.TokenRevokedSuccessfully])
            };
        }

        public async Task<ApiResponse> Handle(SendForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.SendForgetPasswordAsync(request.Email);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileSavingTheCode" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSavingTheCode]),
                "AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain]),
                _ => Success(null, message: stringLocalizer[SharedTranslationKeys.ForgetPasswordEmailHasBeenSent])
            };
        }

        public async Task<ApiResponse> Handle(ForgetPasswordConfirmationCommand request, CancellationToken cancellationToken)
        {
            var (result, token) = await authenticationService.ForgetPasswordConfirmationAsync(request.Email, request.Code);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "TheCodeHasExpired" => BadRequest(stringLocalizer[SharedTranslationKeys.TheCodeHasExpired]),
                "TheCodeEnteredIsIncorrect" => BadRequest(stringLocalizer[SharedTranslationKeys.TheCodeEnteredIsIncorrect]),
                "AnErrorOccurredWhileDeletingTheCode" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheCode]),
                "AnErrorOccurredWhileSavingThePasswordresetToken" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSavingThePasswordresetToken]),
                _ => Success(new
                {
                    Email = request.Email,
                    Token = token
                }, message: stringLocalizer[SharedTranslationKeys.TheCodeHasbeenVerified])
            };
        }

        public async Task<ApiResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.ResetPasswordAsync(request.Email, request.Password, request.Token);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileDeletingTheOldPassword" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheOldPassword]),
                "AnErrorOccurredWhileAddingTheNewPassword" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheNewPassword]),
                "AnErrorOccurredWhileChangingThePassword" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingThePassword]),
                "InvalidPasswordResetToken" => BadRequest(stringLocalizer[SharedTranslationKeys.InvalidPasswordResetToken]),
                "ThereWasAProblemDeletingThePasswordResetToken" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingThePasswordResetToken]),
                _ => Success(null, message: stringLocalizer[SharedTranslationKeys.PasswordChangedSuccessfully])
            };
        }
    }
}
