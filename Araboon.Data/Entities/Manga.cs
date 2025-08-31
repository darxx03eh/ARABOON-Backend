namespace Araboon.Data.Entities
{
    public class Manga
    {
        public int MangaID { get; set; }
        public string MangaNameEn { get; set; }
        public string MangaNameAr { get; set; }
        public string? AuthorEn { get; set; }
        public string? AuthorAr { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string StatusEn { get; set; }
        public string StatusAr { get; set; }
        public string TypeEn { get; set; }
        public string TypeAr { get; set; }
        public string? CoverImage { get; set; }
        public string? MainImage { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public Double? Rate { get; set; }
        public bool? ArabicAvailable { get; set; } = true;
        public bool? EnglishAvilable { get; set; } = false;
        public int? RatingsCount { get; set; }
        public virtual ICollection<Favorite>? Favorites { get; set; } = new HashSet<Favorite>();
        public virtual ICollection<CompletedReads>? CompletedReads { get; set; } = new HashSet<CompletedReads>();
        public virtual ICollection<CurrentlyReading>? CurrentlyReadings { get; set; } = new HashSet<CurrentlyReading>();
        public virtual ICollection<ReadingLater>? ReadingLaters { get; set; } = new HashSet<ReadingLater>();
        public virtual ICollection<Notifications>? Notifications { get; set; } = new HashSet<Notifications>();
        public virtual ICollection<CategoryManga>? CategoryMangas { get; set; } = new HashSet<CategoryManga>();
        public virtual ICollection<Chapter>? Chapters { get; set; } = new HashSet<Chapter>();
        public virtual ICollection<Comment>? Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<ChapterView>? ChapterViews { get; set; } = new HashSet<ChapterView>();
        public virtual ICollection<Ratings>? Ratings { get; set; } = new HashSet<Ratings>();
    }
}
