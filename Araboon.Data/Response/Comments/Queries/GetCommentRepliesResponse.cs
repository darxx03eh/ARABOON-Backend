using Araboon.Data.Response.Users.Queries;

namespace Araboon.Data.Response.Comments.Queries
{
    public class GetCommentRepliesResponse
    {
        public FromUser From { get; set; }
    }
    public class FromUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public ToUser To { get; set; }
        public UserReply? Reply { get; set; }
    }
    public class ToUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
    public class UserReply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Since { get; set; }
        public int? Likes { get; set; }
        public bool IsLike { get; set; }
    }
}
