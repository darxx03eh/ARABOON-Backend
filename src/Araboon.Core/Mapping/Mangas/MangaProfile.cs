using Araboon.Infrastructure.Commons;
using AutoMapper;

namespace Araboon.Core.Mapping.Mangas
{
    public partial class MangaProfile : Profile
    {
        public MangaProfile( )
        {
            GetMangaByIDMapping();
        }
    }
}
