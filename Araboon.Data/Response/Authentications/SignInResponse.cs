namespace Araboon.Data.Response.Authentications
{
    public class SignInResponse
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
    public class RefreshToken
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
