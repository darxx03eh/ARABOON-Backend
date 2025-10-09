using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Ratings
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int MangaID { get; set; }
        public double Rate { get; set; }
        public virtual AraboonUser? User { get; set; }
        public virtual Manga? Manga { get; set; }
    }
}
