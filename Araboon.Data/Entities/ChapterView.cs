using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class ChapterView
    {
        public int UserID { get; set; }
        public int MangaID { get; set; }
        public int ChapterID { get; set; }
        public DateTime? ViewAt { get; set; } = DateTime.UtcNow;
        public virtual AraboonUser? User { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual Chapter? Chapter { get; set; }
    }
}
