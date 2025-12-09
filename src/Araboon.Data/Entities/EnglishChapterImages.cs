namespace Araboon.Data.Entities
{
    public class EnglishChapterImages
    {
        public int ImageID { get; set; }
        public int ChapterID { get; set; }
        public string ImageUrl { get; set; }
        public int OrderImage { get; set; }
        public virtual Chapter? Chapter { get; set; }
    }
}
