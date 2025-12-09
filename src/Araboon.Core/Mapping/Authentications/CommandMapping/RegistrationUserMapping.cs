using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Data.Entities.Identity;

namespace Araboon.Core.Mapping.Authentications
{
    public partial class AuthenticationProfile
    {
        public void RegistrationUserMapping()
        {
            CreateMap<RegistrationUserCommand, AraboonUser>()
                .ForMember(to => to.FirstName, from => from.MapFrom(user => user.FirstName))
                .ForMember(to => to.LastName, from => from.MapFrom(user => user.LastName))
                .ForMember(to => to.Email, from => from.MapFrom(user => user.Email))
                .ForMember(to => to.UserName, from => from.MapFrom(user => user.UserName));
        }
    }
}
