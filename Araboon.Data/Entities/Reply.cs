using Araboon.Data.Entities.Identity;

namespace Araboon.Data.Entities
{
    public class Reply
    {
        public Int32 ReplyID { get; set; }
        public Int32 UserID { get; set; }
        public Int32 CommentID { get; set; }
        public String Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Int32? Likes { get; set; }
        public Int32? DisLikes { get; set; }
        public virtual AraboonUser? User { get; set; }
        public virtual Comment? Comment { get; set; }
    }
}
