using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace BookStore.Utility;

public class MailKitEmailSender : IEmailSender
{
    public AuthMessageSenderOptions Options { get; } 
    
    public MailKitEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
    {
        Options = optionsAccessor.Value;
    }
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailToSend = new MimeMessage();
        emailToSend.From.Add(MailboxAddress.Parse(Options.GmailAddress));
        emailToSend.To.Add(MailboxAddress.Parse(email));
        emailToSend.Subject = subject;
        emailToSend.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

        using (var smtp = new SmtpClient())
        {
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(Options.GmailAddress, Options.GmailPassword);
            smtp.Send(emailToSend);
            smtp.Disconnect(true);
        }
        
        return Task.CompletedTask;
    }
}