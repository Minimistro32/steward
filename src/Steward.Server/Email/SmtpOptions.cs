namespace Steward.Server.Email;

public sealed class SmtpOptions
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool UseStartTls { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "";
    public string FromName { get; set; } = "Steward";
    public bool IsConfigured => Enabled && !string.IsNullOrWhiteSpace(Host)
        && Port is > 0 and <= 65535 && System.Net.Mail.MailAddress.TryCreate(FromAddress, out _)
        && (string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password));
}
