namespace Araboon.Data.Entities
{
    public class CategoryManga
    {
        public Int32 CategoryID { get; set; }
        public Int32 MangaID { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual Category? Category { get; set; }
    }
}
