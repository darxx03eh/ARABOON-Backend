using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        public Task<(String, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(Int32 mangaId, String language);
    }
}
