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

        public async Task<(string, int?)> ChapterReadAsync(int chapterId)
        {
            var chapter = await chapterRepository.GetByIdAsync(chapterId);
            if (chapter is null)
                return ("ChapterNotFound", null);

            try
            {
                chapter.ReadersCount++;
                await chapterRepository.UpdateAsync(chapter);
                return ("ViewsIncreasedBy1", chapter.ReadersCount);
            }catch(Exception exp)
            {
                return ("AnErrorOccurredWhileIncreasingTheViewByOne", null);
            }
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
