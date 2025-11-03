using Microsoft.AspNetCore.Http;

namespace Araboon.Data.DTOs.Chapters
{
    public class ChapterInfoDTO
    {
        public int MangaId { get; set; }
        public int ChapterNo { get; set; }
        public string? ArabicChapterTitle { get; set; } 
        public string? EnglishChapterTitle { get; set; } 
        public IFormFile Image { get; set; }
        public string Language { get; set; }
        public IList<IFormFile> ChapterImages { get; set; }
    }
}
