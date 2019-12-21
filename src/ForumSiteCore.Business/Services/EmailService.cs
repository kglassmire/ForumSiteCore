using ForumSiteCore.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ForumSiteCore.Business.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine("Woo sent an email.");
            return Task.FromResult(0);
        }
    }
}
