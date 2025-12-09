using Araboon.Data.Entities;
using Araboon.Data.Response.Mangas.Queries;
using AutoMapper;

namespace Araboon.Infrastructure.Resolvers.MangasResolver
{
    public class CommentsCountResolver : IValueResolver<Manga, GetMangaByIDResponse, int>
    {
        public int Resolve(Manga source, GetMangaByIDResponse destination, int destMember, ResolutionContext context)
            => context.Items.TryGetValue("CommentsCount", out var value) ? (int)value : 0;
    }
}
