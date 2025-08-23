using Araboon.Data.Entities;
namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetMangaByIDResponse
    {
        public string MangaName { get; set; }
        public Double? Rate { get; set; }
        public bool IsFavorite { get; set; } = false;
        public bool IsCompletedReading { get; set; } = false;
        public bool IsCurrentlyReading { get; set; } = false;
        public bool IsReadingLater { get; set; } = false;
        public bool IsNotification { get; set; } = false;
        public bool IsArabicAvailable { get; set; } = true;
        public bool IsEnglishAvailable { get; set; } = false;
        public string Author { get; set; }
        public string MangaImageUrl { get; set; }
        public IList<string> Categories { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string PublishedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string Description { get; set; }
    }
}
