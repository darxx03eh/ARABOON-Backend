namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetCategoriesHomePageResponse
    {
        public Int32? MangaID { get; set; }
        public String? MangaName { get; set; }
        public String? MangaImageUrl { get; set; }
        public LastChapter? LastChapter { get; set; }
        public Boolean? IsFavorite { get; set; } = false;
    }
    public class LastChapter
    {
        public Int32 ChapterID { get; set; }
        public Int32 ChapterNo { get; set; }
        public Int32? Views { get; set; }
    }
}
