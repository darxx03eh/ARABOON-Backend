namespace Araboon.Data.Entities
{
    public class Chapter
    {
        public int ChapterID { get; set; }
        public int MangaID { get; set; }
        public int ChapterNo { get; set; }
        public string? ArabicChapterTitle { get; set; }
        public string? EnglishChapterTitle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }
        public int? ReadersCount { get; set; }
        public string? Language { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual ICollection<ArabicChapterImages>? ArabicChapterImages { get; set; } = new HashSet<ArabicChapterImages>();
        public virtual ICollection<EnglishChapterImages>? EnglishChapterImages { get; set; } = new HashSet<EnglishChapterImages>();
        public virtual ICollection<ChapterView>? ChapterViews { get; set; } = new HashSet<ChapterView>();
    }
}
