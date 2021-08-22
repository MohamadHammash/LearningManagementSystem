using System.Threading.Tasks;

using FluentEmail.Core;

using Microsoft.AspNetCore.Identity.UI.Services;

namespace Lms.MVC.UI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IFluentEmail singleEmail;

        public EmailSender(IFluentEmail singleEmail)
        {
            this.singleEmail = singleEmail;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = singleEmail
                .To(email)
                .Subject(subject)
                .Body(htmlMessage, true);

            await emailToSend.SendAsync();
        }
    }
}