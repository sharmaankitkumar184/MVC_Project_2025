using MVC_Project.Services.Services.IService;
using System.Net;
using System.Net.Mail;

namespace MVC_Project.Services.Services
{
    public class EmailService :IEmailService
    {

        public async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("masterprojectsmailer@gmail.com"),
                Subject = "Reset Your Password",
                Body = $@"
                <p>You requested a password reset.</p>
                <p>
                    <a href='{resetLink}'>
                        Click here to reset your password
                    </a>
                </p>
                <p>This link will expire in 10 minutes.</p>",
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    "masterprojectsmailer@gmail.com",
                    "ezmfnwqxrvidjveq"
                ),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
