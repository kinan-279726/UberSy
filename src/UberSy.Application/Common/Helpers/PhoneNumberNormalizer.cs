namespace UberSy.Application.Common.Helpers;

public class PhoneNumberNormalizer
{
    // E.164
    public static string? ToE164(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var trimmed = raw.Trim();
        var hasLeadingPlus = trimmed.StartsWith('+');
        var digits = new string(trimmed.Where(char.IsDigit).ToArray());

        if (digits.Length == 0) return null;

        // 00 → +
        if (digits.StartsWith("00"))
        {
            digits = digits[2..];
            hasLeadingPlus = true;
        }

        // 09xxxxxxxx
        if (!hasLeadingPlus && digits.StartsWith("09") && digits.Length == 10)
            return "+963" + digits[1..];

        // 9xxxxxxxx (9 أرقام) → +9639xxxxxxxx
        if (!hasLeadingPlus && digits.StartsWith('9') && digits.Length == 9)
            return "+963" + digits;

        if (hasLeadingPlus && digits.StartsWith("9639") && digits.Length == 12)
            return "+" + digits;

        return null;
    }
}
