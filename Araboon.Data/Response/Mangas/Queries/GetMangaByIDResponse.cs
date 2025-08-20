using Araboon.Data.Entities;
namespace Araboon.Data.Response.Mangas.Queries
{
    public class GetMangaByIDResponse
    {
        public String MangaName { get; set; }
        public Double? Rate { get; set; }
        public Boolean IsFavorite { get; set; } = false;
        public Boolean IsCompletedReading { get; set; } = false;
        public Boolean IsCurrentlyReading { get; set; } = false;
        public Boolean IsReadingLater { get; set; } = false;
        public Boolean IsNotification { get; set; } = false;
        public Boolean IsArabicAvailable { get; set; } = true;
        public Boolean IsEnglishAvailable { get; set; } = false;
        public String Author { get; set; }
        public String MangaImageUrl { get; set; }
        public IList<String> Categories { get; set; }
        public String Status { get; set; }
        public String Type { get; set; }
        public String PublishedOn { get; set; }
        public String UpdatedOn { get; set; }
        public String Description { get; set; }
    }
}
