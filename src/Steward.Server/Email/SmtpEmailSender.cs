using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Steward.Server.Email;

public sealed class SmtpEmailSender(IOptions<SmtpOptions> options)
{
    public bool IsConfigured => options.Value.IsConfigured;

    public async Task SendRecoveryPinAsync(string recipient, string pin, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.UseStartTls,
            UseDefaultCredentials = false,
            Credentials = string.IsNullOrEmpty(settings.Username) ? null : new NetworkCredential(settings.Username, settings.Password)
        };
        using var message = new MailMessage
        {
            From = new MailAddress(settings.FromAddress, settings.FromName),
            Subject = "Your Steward sign-in PIN",
            Body = $"Your new Steward PIN is: {pin}\n\nUse it on your Steward login page within 30 minutes. Once you sign in with it, it becomes your PIN and your previous PIN stops working. You can change it in your account afterwards.\n\nIf you did not request this, ignore this email. Your existing PIN still works until this new PIN is used.",
            IsBodyHtml = false
        };
        message.To.Add(recipient);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        await client.SendMailAsync(message, timeout.Token);
    }
}
