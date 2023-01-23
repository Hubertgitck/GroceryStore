using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Application.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            /*var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("hello@grocery.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text= htmlMessage };

            //send email
            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smt.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("grocery@gmail.com", "DotNet213$");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }*/

            return Task.CompletedTask;
        }
    }
}
