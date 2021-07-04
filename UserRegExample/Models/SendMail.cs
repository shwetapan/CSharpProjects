using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace UserRegExample.Models
{
    public class SendMail : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using(MailMessage mailMessage=new MailMessage())
            {
                mailMessage.From = new MailAddress("7spandey@gmail.com");
                mailMessage.Subject = subject;
                mailMessage.Body = email + htmlMessage;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(email));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                System.Net.NetworkCredential NetworkCread = new System.Net.NetworkCredential();
                NetworkCread.UserName = "7spandey@gmail.com";
                NetworkCread.Password = "7Shw82t3#p";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCread;
                smtp.Port = 25;
                await smtp.SendMailAsync(mailMessage);
                    
            }
        }
    }
}
