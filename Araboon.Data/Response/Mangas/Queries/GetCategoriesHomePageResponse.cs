using System.Text.Json.Serialization;

namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetCategoriesHomePageResponse
    {
        public int? MangaID { get; set; }
        public string? MangaName { get; set; }
        public string? MangaImageUrl { get; set; }
        public LastChapter? LastChapter { get; set; }
        public bool? IsFavorite { get; set; } = false;
    }
    public class LastChapter
    {
        public int ChapterID { get; set; }
        public int ChapterNo { get; set; }
        public int? Views { get; set; }
    }
}
