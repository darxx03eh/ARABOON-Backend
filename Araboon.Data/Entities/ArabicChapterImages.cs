namespace Araboon.Data.Entities
{
    public class ArabicChapterImages
    {
        public Int32 ImageID { get; set; }
        public Int32 ChapterID { get; set; }
        public String ImageUrl { get; set; }
        public Int32 Order { get; set; }
        public virtual Chapter? Chapter { get; set; }
    }
}
