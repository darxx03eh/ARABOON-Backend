namespace Araboon.Data.Entities
{
    public class Manga
    {
        public Int32 MangaID { get; set; }
        public String MangaNameEn { get; set; }
        public String MangaNameAr { get; set; }
        public String? AuthorEn { get; set; }
        public String? AuthorAr { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public String StatusEn { get; set; }
        public String StatusAr { get; set; }
        public String TypeEn { get; set; }
        public String TypeAr { get; set; }
        public String? CoverImage { get; set; }
        public String? MainImage { get; set; }
        public String? DescriptionEn { get; set; }
        public String? DescriptionAr { get; set; }
        public Double? Rate { get; set; }
        public Boolean? ArabicAvailable { get; set; } = true;
        public Boolean? EnglishAvilable { get; set; } = false;
        public Int32? RatingsCount { get; set; }
        public virtual ICollection<Favorite>? Favorites { get; set; } = new HashSet<Favorite>();
        public virtual ICollection<CompletedReads>? CompletedReads { get; set; } = new HashSet<CompletedReads>();
        public virtual ICollection<CurrentlyReading>? CurrentlyReadings { get; set; } = new HashSet<CurrentlyReading>();
        public virtual ICollection<ReadingLater>? ReadingLaters { get; set; } = new HashSet<ReadingLater>();
        public virtual ICollection<Notifications>? Notifications { get; set; } = new HashSet<Notifications>();
        public virtual ICollection<CategoryManga>? CategoryMangas { get; set; } = new HashSet<CategoryManga>();
        public virtual ICollection<Chapter>? Chapters { get; set; } = new HashSet<Chapter>();
        public virtual ICollection<Comment>? Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<ChapterView>? ChapterViews { get; set; } = new HashSet<ChapterView>();
    }
}
