using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Comment
    {
        public Int32 CommentID { get; set; }
        public Int32 UserID { get; set; }
        public Int32 MangaID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public String Content { get; set; }
        public Int32? Likes { get; set; }
        public Int32? DisLikes { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual AraboonUser? User { get; set; }
        public virtual ICollection<Reply>? Replies { get; set; } = new HashSet<Reply>();
    }
}
