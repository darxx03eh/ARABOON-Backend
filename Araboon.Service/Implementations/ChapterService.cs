using Araboon.Data.Entities;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository chapterRepository;
        public ChapterService(IChapterRepository chapterRepository)
        {
            this.chapterRepository = chapterRepository;
        }
        public async Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language)
        {
            var (message, chapters) = await chapterRepository.GetChaptersForSpecificMangaByLanguage(mangaId, language);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "TheLanguageYouRequestedIsNotAvailableForThisManga" => ("TheLanguageYouRequestedIsNotAvailableForThisManga", null),
                "ThereAreNoChaptersYet" => ("ThereAreNoChaptersYet", null),
                "TheChaptersWereFound" => ("TheChaptersWereFound", chapters),
                _ => ("ThereAreNoChaptersYet", null)
            };
        }
    }
}
