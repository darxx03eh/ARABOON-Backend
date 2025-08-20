namespace Araboon.Data.Entities
{
    public class Category
    {
        public Int32 CategoryID { get; set; }
        public String CategoryNameEn { get; set; }
        public String CategoryNameAr { get; set; }
        public virtual ICollection<CategoryManga> CategoryMangas { get; set; } = new HashSet<CategoryManga>();
    }
}
