namespace Araboon.Data.Entities
{
    public class Chapter
    {
        public Int32 ChapterID { get; set; }
        public Int32 MangaID { get; set; }
        public Int32 ChapterNo { get; set; }
        public String? ArabicChapterTitle { get; set; }
        public String? EnglishChapterTitle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public String? ImageUrl { get; set; }
        public Int32? ReadersCount { get; set; }
        public String? Language { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual ICollection<ArabicChapterImages>? ArabicChapterImages { get; set; } = new HashSet<ArabicChapterImages>();
        public virtual ICollection<EnglishChapterImages>? EnglishChapterImages { get; set; } = new HashSet<EnglishChapterImages>();
        public virtual ICollection<ChapterView>? ChapterViews { get; set; } = new HashSet<ChapterView>();
    }
}
