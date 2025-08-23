namespace Araboon.Data.Entities
{
    public class CategoryManga
    {
        public int CategoryID { get; set; }
        public int MangaID { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual Category? Category { get; set; }
    }
}
