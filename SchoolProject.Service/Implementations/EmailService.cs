using MailKit.Net.Smtp;
using MimeKit;
using SchoolProject.Data.Helpers;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class EmailService : IEmailService
    {
        #region Fields
        private readonly EmailSettings emailSettings;
        #endregion

        #region Constructor
        public EmailService(EmailSettings emailSettings)
        {
            this.emailSettings = emailSettings;
        }
        #endregion

        #region Functions
        public async Task<string> SendEmail(string email, string message, string? reason)
        {
            try
            {
                //sending the Message of passwordResetLink
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSettings.Host, emailSettings.Port, true);
                    client.Authenticate(emailSettings.FromEmail, emailSettings.Password);
                    var bodybuilder = new BodyBuilder
                    {
                        HtmlBody = $"{message}",
                        TextBody = "wellcome",
                    };
                    var Message = new MimeMessage
                    {
                        Body = bodybuilder.ToMessageBody()
                    };
                    Message.From.Add(new MailboxAddress("Future Team", emailSettings.FromEmail));
                    Message.To.Add(new MailboxAddress("testing", email));
                    Message.Subject = reason == null ? "No Submitted" : reason;
                    await client.SendAsync(Message);
                    await client.DisconnectAsync(true);
                }
                //end of sending email
                return "Success";
            }
            catch
            {
                return "Failed";
            }
        }
        #endregion
    }
}
