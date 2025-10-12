namespace Araboon.Data.Response.ChapterImages
{
    public class ChapterImagesResponse
    {
        public int ChapterId { get; set; }
        public bool IsArabic { get; set; }
        public bool IsEnglish { get; set; }
        public IList<string> Images { get; set; }
    }
}
