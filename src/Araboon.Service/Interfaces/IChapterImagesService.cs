using Araboon.Data.Response.ChapterImages;

namespace Araboon.Service.Interfaces
{
    public interface IChapterImagesService
    {
        public Task<(string, ChapterImagesResponse?, string?, int?)> GetChapterImagesAsync(int mangaId, int chapterNo, string language);
    }
}
