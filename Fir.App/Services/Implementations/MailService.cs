using Fir.App.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Fir.App.Services.Implementations
{
    public class MailService : IMailService
    {
        public async Task Send(string from, string to,string link,string subject)
        {

            MailMessage mm = new MailMessage();
            mm.From = new MailAddress(from);
            mm.To.Add(to);
            mm.Subject = subject;
            mm.Body = $"<a href='{link}'>Click me</a>";
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("hajarih@code.edu.az", "butbvpedpqrhnfea");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(mm);
        }
    }
}
