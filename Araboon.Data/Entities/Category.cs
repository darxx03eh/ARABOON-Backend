namespace Araboon.Data.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
        public virtual ICollection<CategoryManga> CategoryMangas { get; set; } = new HashSet<CategoryManga>();
    }
}
