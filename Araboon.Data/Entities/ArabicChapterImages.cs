namespace Araboon.Data.Entities
{
    public class ArabicChapterImages
    {
        public int ImageID { get; set; }
        public int ChapterID { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
        public virtual Chapter? Chapter { get; set; }
    }
}
