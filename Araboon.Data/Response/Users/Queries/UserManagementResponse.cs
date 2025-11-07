namespace Araboon.Data.Response.Users.Queries
{
    public class UserManagementResponse
    {
        public int Id { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role {  get; set; }
        public string? CreatedAt { get; set; }
        public string LastLogin { get; set; }
        public bool IsActive { get; set; }
    }
}
