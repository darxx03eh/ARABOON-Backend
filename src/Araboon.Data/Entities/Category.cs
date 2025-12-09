namespace Araboon.Data.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<CategoryManga> CategoryMangas { get; set; } = new HashSet<CategoryManga>();
    }
}
