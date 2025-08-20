namespace Araboon.Data.Response.Authentications
{
    public class SignInResponse
    {
        public String AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
    public class RefreshToken
    {
        public String UserName { get; set; }
        public String Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
