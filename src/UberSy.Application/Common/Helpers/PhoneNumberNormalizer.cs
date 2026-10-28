namespace UberSy.Application.Common.Helpers;

public class PhoneNumberNormalizer
{
    // E.164
    public static string? ToE164(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var cleaned = new string(raw.Where(c => char.IsDigit(c) || c == '+').ToArray());

        if (cleaned.StartsWith("00")) cleaned = "+" + cleaned[2..];
        if (cleaned.StartsWith("09") && cleaned.Length == 10) return "+963" + cleaned[1..];
        if (cleaned.StartsWith('9') && cleaned.Length == 9) return "+963" + cleaned;
        if (cleaned.StartsWith('+') && cleaned.Length is >= 9 and <= 16) return cleaned;

        return null;
    }
}
