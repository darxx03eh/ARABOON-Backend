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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CategoryResponse? Category { get; set; }
    }
    public class LastChapter
    {
        public int ChapterID { get; set; }
        public int ChapterNo { get; set; }
        public int? Views { get; set; }
    }
    public class CategoryResponse
    {
        public string En { get; set; }
        public string Ar { get; set; }
    }
}
