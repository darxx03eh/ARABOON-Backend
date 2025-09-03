using Araboon.Data.Helpers;

namespace Araboon.Data.Entities.Identity
{
    public class UserRefreshToken
    {
        public int ID { get; set; }
        public string Jti { get; set; }
        public int UserID { get; set; }
        private string? token;
        public string? Token
        {
            get => token is null ? null : EncryptionHelper.Decrypt(token);
            set => token = value is null ? null : EncryptionHelper.Encrypt(value);
        }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime ExpirydDate { get; set; }
        public virtual AraboonUser? User { get; set; }
    }
}
