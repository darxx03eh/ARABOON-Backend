using Araboon.Data.Entities;
using Araboon.Infrastructure.IRepositories;
using Araboon.Infrastructure.Repositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class ChapterViewService : IChapterViewService
    {
        private readonly IChapterViewRepository chapterViewRepository;
        private readonly IMangaRepository mangaRepository;

        public ChapterViewService(IChapterViewRepository chapterViewRepository, IMangaRepository mangaRepository)
        {
            this.chapterViewRepository = chapterViewRepository;
            this.mangaRepository = mangaRepository;
        }
        public async Task<String> MarkAsReadAsync(Int32 mangaId, Int32 chapterId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = chapterViewRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "MarkAsReadForChaptersServiceforRegisteredUsersOnly";
                var exist = await chapterViewRepository.IsMangaAndChapterExistForUser(mangaId, chapterId, Int32.Parse(userId));
                if (exist)
                    return "ThisChapterInThisMangaIsAlreadyMarkedAsRead";
                var chapterExistInManga = await chapterViewRepository.IsChapterExistInManga(chapterId, mangaId);
                if (!chapterExistInManga)
                    return "ThisChapterIsNotInThisManga";
                await chapterViewRepository.AddAsync(new ChapterView()
                {
                    MangaID = mangaId,
                    ChapterID = chapterId,
                    UserID = Int32.Parse(userId)
                });
                return "MarkedAsRead";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemMarkedAsRead";
            }
        }

        public async Task<String> MarkAsUnReadAsync(Int32 mangaId, Int32 chapterId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = chapterViewRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "MarkAsUnReadForChaptersServiceforRegisteredUsersOnly";
                var exist = await chapterViewRepository.IsMangaAndChapterExistForUser(mangaId, chapterId, Int32.Parse(userId));
                if (!exist)
                    return "ThisChapterForThisMangaIsNotExistInMarkedAsRead";
                await chapterViewRepository.DeleteAsync(new ChapterView()
                {
                    MangaID = mangaId,
                    ChapterID = chapterId,
                    UserID = Int32.Parse(userId)
                });
                return "MarkedAsUnRead";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemMarkedAsUnRead";
            }
        }
    }
}
