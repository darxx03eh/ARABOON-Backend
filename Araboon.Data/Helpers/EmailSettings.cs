namespace Araboon.Data.Helpers
{
    public class EmailSettings
    {
        public String FromEmail { get; set; }
        public String Password { get; set; }
        public String SmtpServer { get; set; }
        public Int32 Port { get; set; }
        public Boolean UseSSL { get; set; }
    }
}
