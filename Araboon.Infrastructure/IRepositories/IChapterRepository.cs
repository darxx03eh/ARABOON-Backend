using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        public Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language);
    }
}
