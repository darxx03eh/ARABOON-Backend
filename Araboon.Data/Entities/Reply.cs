using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Reply
    {
        public int ReplyID { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public int CommentID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? Likes { get; set; }
        public virtual AraboonUser? FromUser { get; set; }
        public virtual AraboonUser? ToUser { get; set; }
        public virtual Comment? Comment { get; set; }
        public virtual ICollection<ReplyLikes> ReplyLikes { get; set; } = new HashSet<ReplyLikes>();
    }
}
