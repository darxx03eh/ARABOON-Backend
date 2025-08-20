namespace Araboon.Data.Helpers
{
    public class JwtSettings
    {
        public String Issuer { get; set; }
        public String Audience { get; set; }
        public String SecretKey { get; set; }
        public Boolean ValidateAudience { get; set; }
        public Boolean ValidateIssuer { get; set; }
        public Boolean ValidateLifetime { get; set; }
        public Boolean ValidateIssuerSigningKey { get; set; }
        public Int32 AccessTokenExpireDate { get; set; }
        public Int32 RefreshTokenExpireDate { get; set; }
    }
}
