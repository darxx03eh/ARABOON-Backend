using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class ChapterView
    {
        public Int32 UserID { get; set; }
        public Int32 MangaID { get; set; }
        public Int32 ChapterID { get; set; }
        public DateTime? ViewAt { get; set; } = DateTime.UtcNow;
        public virtual AraboonUser? User { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual Chapter? Chapter { get; set; }
    }
}
