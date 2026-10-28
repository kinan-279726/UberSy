namespace UberSy.Infrastructure.Settings;

public sealed class SmsSettings
{
    public string AccountSid { get; set; } = null!;
    public string AuthToken { get; set; } = null!;
    public string FromNumber { get; set; } = null!;
}