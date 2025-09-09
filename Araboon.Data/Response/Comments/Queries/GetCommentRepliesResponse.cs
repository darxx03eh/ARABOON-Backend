using Araboon.Data.Response.Users.Queries;

namespace Araboon.Data.Response.Comments.Queries
{
    public class GetCommentRepliesResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Since { get; set; }
        public int? Likes { get; set; }
        public bool IsLiked { get; set; }
        public FromUser User { get; set; }
        public ToUser ReplyToUser { get; set; }
    }
    public class FromUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public ProfileImage? ProfileImage { get; set; }
    }
    public class ToUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
