using Nabta_Production.DAL;
using System.Net;
using System.Net.Mail;

namespace Nabta_Production.BL
{
    public class EmailManager : IEmailManager
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailManager(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public  async Task<bool> SendEmail(string recieverEmail, string subject, string message)
        {
            try
            {
                SmtpClient smtpClient = new(_emailConfiguration.SmtpServer, _emailConfiguration.Port)
                {
                    //EnableSsl = true,
                    Credentials = new NetworkCredential(_emailConfiguration.UserName, _emailConfiguration.Password)
                };

              //  smtpClient.Timeout = TimeSpan.FromSeconds(1);
                /*var result =*/ await smtpClient.SendMailAsync(new MailMessage(
                                            from:_emailConfiguration.EmailSender,
                                            to:recieverEmail,
                                            subject:subject,
                                            body:message
                                            ));
                //if (!result.IsCompletedSuccessfully)
                //{
                //    var ex = result.Status;
                    

                //    return false;
                //}

                return true;
            }
            catch (Exception ex)
            {
                var messageException = ex.Message;
                return false;
            }
        }
    }
}
