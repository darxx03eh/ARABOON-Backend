using Araboon.Data.Entities;

namespace Araboon.Service.Interfaces
{
    public interface IChapterService
    {
        public Task<(String, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(Int32 mangaId, String language);
    }
}
