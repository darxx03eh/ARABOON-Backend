using Araboon.Core.Bases;
using Araboon.Core.Features.Authentications.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Queries.Handlers
{
    public class AuthenticationQueryHandler : ApiResponseHandler
        , IRequestHandler<ValidateAccessTokenQuery, ApiResponse>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IAuthenticationService authenticationService;

        public AuthenticationQueryHandler(IStringLocalizer<SharedTranslation> stringLocalizer, IAuthenticationService authenticationService)
            : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authenticationService = authenticationService;
        }

        public async Task<ApiResponse> Handle(ValidateAccessTokenQuery request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.ValidateAccessToken(request.AccessToken);
            return result switch
            {
                "InvalidTokenFormat" => BadRequest(stringLocalizer[SharedTranslationKeys.InvalidTokenFormat]),
                "TokenExpired" => BadRequest(stringLocalizer[SharedTranslationKeys.TokenExpired]),
                "AnErrorOccurredWhileVerifyingTheToken" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileVerifyingTheToken]),
                "InvalidToken" => Unauthorized(stringLocalizer[SharedTranslationKeys.InvalidToken]),
                "ValidToken" => Success(null, message: stringLocalizer[SharedTranslationKeys.ValidToken]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileVerifyingTheToken])
            };
        }
    }
}
