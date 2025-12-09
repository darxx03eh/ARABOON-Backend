namespace Araboon.Service.Interfaces
{
    public interface IChapterViewService
    {
        public Task<string> MarkAsReadAsync(int mangaId, int chapterId);
        public Task<string> MarkAsUnReadAsync(int mangaId, int chapterId);
    }
}
