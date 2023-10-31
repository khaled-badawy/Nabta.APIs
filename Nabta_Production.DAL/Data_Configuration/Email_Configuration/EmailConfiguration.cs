namespace Nabta_Production.DAL
{
    public class EmailConfiguration
    {
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SmtpServer { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailSender { get; set; } = string.Empty;
    }
}
