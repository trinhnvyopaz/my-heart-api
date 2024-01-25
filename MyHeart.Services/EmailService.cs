using MyHeart.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _configuration;

        public EmailService(IEmailConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMail(string to, string subject, string body)
        {
            using var emailClient = new SmtpClient();
            //The last parameter here is to use SSL (Which you should!)
            await emailClient.ConnectAsync(_configuration.SmtpServer, _configuration.SmtpPort, _configuration.UseSSL);

            //Remove any OAuth functionality as we won't be using it.
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            await emailClient.AuthenticateAsync(_configuration.SmtpUsername, _configuration.SmtpPassword);

            var message = new MimeMessage();

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();
            message.Subject = subject;
            message.From.Add(new MailboxAddress(_configuration.From));
            message.To.Add(new MailboxAddress(to));

            await emailClient.SendAsync(message);
            await emailClient.DisconnectAsync(true);
        }

        public async Task SendAdminMail(string subject, string body) {
            using var emailClient = new SmtpClient();
            //The last parameter here is to use SSL (Which you should!)
            await emailClient.ConnectAsync(_configuration.SmtpServer, _configuration.SmtpPort, _configuration.UseSSL);

            //Remove any OAuth functionality as we won't be using it.
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            await emailClient.AuthenticateAsync(_configuration.SmtpUsername, _configuration.SmtpPassword);

            var message = new MimeMessage();

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();
            message.Subject = subject;
            message.From.Add(new MailboxAddress(_configuration.From));
            message.To.Add(new MailboxAddress(_configuration.AdminEmail));

            await emailClient.SendAsync(message);
            await emailClient.DisconnectAsync(true);
        }
    }
}