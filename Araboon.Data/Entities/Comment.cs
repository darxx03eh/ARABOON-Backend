using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int UserID { get; set; }
        public int MangaID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Content { get; set; }
        public int? Likes { get; set; }
        public virtual Manga? Manga { get; set; }
        public virtual AraboonUser? User { get; set; }
        public virtual ICollection<Reply>? Replies { get; set; } = new HashSet<Reply>();
        public virtual ICollection<CommentLikes> CommentLikes { get; set; } = new HashSet<CommentLikes>();
    }
}
