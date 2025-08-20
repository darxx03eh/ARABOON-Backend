namespace Araboon.Data.Entities.Identity
{
    public class UserRefreshToken
    {
        public Int32 ID { get; set; }
        public Int32 UserID { get; set; }
        public String? Token { get; set; }
        public String? RefreshToken { get; set; }
        public String? JwtID { get; set; }
        public Boolean IsUsed { get; set; }
        public Boolean IsRevoked { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime ExpirydDate { get; set; }
        public virtual AraboonUser? User { get; set; }
    }
}
