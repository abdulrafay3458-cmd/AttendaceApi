using AttendanceAPI.Contracts.Request;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

using Mailjet.Client.Resources;


public class EmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmail(string toEmail, string subject, string body)
    {
        try
        {         
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            message.To.Add(new MailboxAddress("test",toEmail));
            message.Subject = subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            message.Body = builder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSettings.ServerOutgoing, _smtpSettings.Port, true);
                await client.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }              
        }
        catch (Exception ex)
        {
            throw new Exception("Email sending failed: " + ex.Message);
        }
    }
}