namespace Araboon.Data.Response.Chapters.Queries
{
    public class ChaptersResponse
    {
        public Int32 ChapterID { get; set; }
        public String Title { get; set; }
        public String? ChapterTitle { get; set; }
        public Boolean IsView { get; set; } = false;
        public String ReleasedOn { get; set; }
        public String ChapterImageUrl { get; set; }
        public Boolean IsArabic { get; set; }
    }
}
