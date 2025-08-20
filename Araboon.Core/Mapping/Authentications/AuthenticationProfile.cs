using AutoMapper;

namespace Araboon.Core.Mapping.Authentications
{
    public partial class AuthenticationProfile : Profile
    {
        public AuthenticationProfile()
        {
            RegistrationUserMapping();
        }
    }
}
