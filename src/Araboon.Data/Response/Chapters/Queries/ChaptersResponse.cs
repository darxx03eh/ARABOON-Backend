namespace Araboon.Data.Response.Chapters.Queries
{
    public class ChaptersResponse
    {
        public int ChapterID { get; set; }
        public string Title { get; set; }
        public string? ChapterTitle { get; set; }
        public string? ChapterTitleAr {  get; set; }
        public string? ChapterTitleEn { get; set; }
        public bool IsView { get; set; } = false;
        public string ReleasedOn { get; set; }
        public string ChapterImageUrl { get; set; }
        public bool IsArabic { get; set; }
    }
}
