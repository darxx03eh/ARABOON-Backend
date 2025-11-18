using Araboon.Core.Bases;
using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Authentications;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Handlers
{
    public class AuthenticationCommandHandler : ApiResponseHandler
        , IRequestHandler<RegistrationUserCommand, ApiResponse>
        , IRequestHandler<ConfirmationEmailCommand, ApiResponse>
        , IRequestHandler<SignInCommand, ApiResponse>
        , IRequestHandler<SendConfirmationEmailCommand, ApiResponse>
        , IRequestHandler<GenerateRefreshTokenCommand, ApiResponse>
        , IRequestHandler<LogOutCommand, ApiResponse>
        , IRequestHandler<SendForgetPasswordCommand, ApiResponse>
        , IRequestHandler<ForgetPasswordConfirmationCommand, ApiResponse>
        , IRequestHandler<ResetPasswordCommand, ApiResponse>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IAuthenticationService authenticationService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationCommandHandler(IStringLocalizer<SharedTranslation> stringLocalizer
                                          , IAuthenticationService authenticationService,
                                            IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authenticationService = authenticationService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
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
                "TheAccountHasBeenCreated" => Created(null, message: stringLocalizer[SharedTranslationKeys.TheAccountHasBeenCreated]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheRegistrationProcess])
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
                "EmailConfirmedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.EmailConfirmedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheEmailConfirmationProcess])
            };
        }

        public async Task<ApiResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var (result, message, refresh) = await authenticationService.SignInAsync(request.UserName, request.Password);
            if(refresh is not null)
            {
                httpContextAccessor.HttpContext.Response.Cookies.Append("refresh", refresh, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
            return message switch
            {
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
                "DataVerifiedAndLogin" => Success(result, message: stringLocalizer[SharedTranslationKeys.DataVerifiedAndLogin]),
                "AnErrorOccurredWhileSavingTheLastLogin" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSavingTheLastLogin]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheLoginProcess])
            };
        }

        public async Task<ApiResponse> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.SendConfirmationEmailAsync(request.UserName);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain]),
                "YourAccountHasAlreadyBeenConfirmed" => BadRequest(stringLocalizer[SharedTranslationKeys.YourAccountHasAlreadyBeenConfirmed]),
                "EmailConfirmationEmailHasBeenSent" => Success(null, message: stringLocalizer[SharedTranslationKeys.EmailConfirmationEmailHasBeenSent]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain])
            };
        }

        public async Task<ApiResponse> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refresh = httpContextAccessor.HttpContext?.Request.Cookies["refresh"];
            if (string.IsNullOrWhiteSpace(refresh))
                return NotFound(stringLocalizer[SharedTranslationKeys.RefreshTokenIsNotFound]);
            var (response, result) = await authenticationService.GenerateRefreshTokenAsync(refresh);
            if(!result.Equals("AccessTokenRegenerated"))
                httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh", new CookieOptions
                {
                    Path = "/",
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            return result switch
            {
                "ErrorInTheEncryptionAlgorithmUsed" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.ErrorInTheEncryptionAlgorithmUsed]),
                "TokenIsStillValidCannotRefreshYet" => BadRequest(stringLocalizer[SharedTranslationKeys.TokenIsStillValidCannotRefreshYet]),
                "RefreshTokenIsNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.RefreshTokenIsNotFound]),
                "RefreshTokenHasExpire" => Unauthorized(stringLocalizer[SharedTranslationKeys.RefreshTokenHasExpire]),
                "RefreshTokenWasRevoked" => Unauthorized(stringLocalizer[SharedTranslationKeys.RefreshTokenWasRevoked]),
                "RefreshTokenNotUsedAnyMore" => Unauthorized(stringLocalizer[SharedTranslationKeys.RefreshTokenNotUsedAnyMore]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredDuringTheTokenGenerationProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheTokenGenerationProcess]),
                "AccessTokenRegenerated" => Success(response, message: stringLocalizer[SharedTranslationKeys.AccessTokenRegenerated]),
                "TheAccountIsDisabledAndCannotBeRegeneratedWithAnAccessToken" =>
                Forbidden(stringLocalizer[SharedTranslationKeys.TheAccountIsDisabledAndCannotBeRegeneratedWithAnAccessToken]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheTokenGenerationProcess])
            };
        }

        public async Task<ApiResponse> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            var refresh = httpContextAccessor.HttpContext?.Request.Cookies["refresh"];
            if (string.IsNullOrWhiteSpace(refresh))
                return NotFound(stringLocalizer[SharedTranslationKeys.RefreshTokenIsNotFound]);
            httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });
            var result = await authenticationService.LogOutAsync(refresh);
            return result switch
            {
                "InvalidRefreshToken" => BadRequest(stringLocalizer[SharedTranslationKeys.InvalidRefreshToken]),
                "AnErrorOccurredDuringTheRefreshTokenCancellationProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheRefreshTokenCancellationProcess]),
                "LogOutSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.LogOutSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheRefreshTokenCancellationProcess])
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
                "ForgetPasswordEmailHasBeenSent" => Success(null, message: stringLocalizer[SharedTranslationKeys.ForgetPasswordEmailHasBeenSent]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain])
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
                "TheCodeHasbeenVerified" => Success(new
                {
                    Email = request.Email,
                    Token = token
                }, message: stringLocalizer[SharedTranslationKeys.TheCodeHasbeenVerified]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemValidatingTheCode])
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
                "PasswordChangedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.PasswordChangedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileChangingThePassword])
            };
        }
    }
}
