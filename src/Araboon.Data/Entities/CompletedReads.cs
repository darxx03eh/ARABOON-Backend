using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class CompletedReads
    {
        public int UserID { get; set; }
        public int MangaID { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual AraboonUser? User { get; set; }
    }
}
