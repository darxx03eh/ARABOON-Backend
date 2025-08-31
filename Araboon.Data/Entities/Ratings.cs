using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Ratings
    {
        public int UserID { get; set; }
        public int MangaID { get; set; }
        public virtual AraboonUser? User { get; set; }
        public virtual Manga? Manga { get; set; }
    }
}
