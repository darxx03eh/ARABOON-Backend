using Araboon.Data.Entities;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Infrastructure.IRepositories;
using AutoMapper;

namespace Araboon.Infrastructure.Resolvers.MangasResolver
{
    public class IsArabicAvilableResolver : IValueResolver<Manga, GetMangaByIDResponse, bool>
    {
        private readonly IUnitOfWork unitOfWork;

        public IsArabicAvilableResolver(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;
        public bool Resolve(Manga source, GetMangaByIDResponse destination, bool destMember, ResolutionContext context)
        {
            var isAdmin = unitOfWork.MangaRepository.IsAdmin().GetAwaiter().GetResult();
            return Convert.ToBoolean(source.ArabicAvailable) || isAdmin;
        }
    }
}
