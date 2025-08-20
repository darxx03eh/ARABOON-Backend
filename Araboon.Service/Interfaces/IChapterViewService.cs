namespace Araboon.Service.Interfaces
{
    public interface IChapterViewService
    {
        public Task<String> MarkAsReadAsync(Int32 mangaId, Int32 chapterId);
        public Task<String> MarkAsUnReadAsync(Int32 mangaId, Int32 chapterId);
    }
}
