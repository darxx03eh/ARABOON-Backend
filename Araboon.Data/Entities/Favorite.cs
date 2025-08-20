using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Favorite
    {
        public Int32 UserID { get; set; }
        public Int32 MangaID { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual AraboonUser? User { get; set; }

    }
}
