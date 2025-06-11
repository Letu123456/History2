using Business.Options;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Service
{
    public class EmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(EmailOptions emailOptions)
        {
            _emailOptions = emailOptions;
        }

        public void SendEmail(string email, string subject, string message)
        {

            var Email = new MimeMessage();
            Email.From.Add(MailboxAddress.Parse(_emailOptions.SenderEmail));
            Email.To.Add(MailboxAddress.Parse(email));
            Email.Subject = subject;

            Email.Body = new TextPart(TextFormat.Html) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect(_emailOptions.Server, _emailOptions.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailOptions.SenderEmail, _emailOptions.Password);

            smtp.Send(Email);
            smtp.Dispose();

        }
    }
}
