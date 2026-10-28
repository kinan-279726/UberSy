namespace UberSy.Application.Common.Helpers;

public static class DamascusTime
{
    private static readonly TimeZoneInfo Zone = Find();
    private static TimeZoneInfo Find()
    {
        foreach (var id in new[] { "Asia/Damascus", "Syria Standard Time" })
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch { }
        return TimeZoneInfo.CreateCustomTimeZone("Damascus", TimeSpan.FromHours(3), "Damascus", "Damascus");
    }
    public static DateOnly Today() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Zone).DateTime);
    public static DateTimeOffset StartOfDayUtc(DateOnly d) =>
        new DateTimeOffset(d.ToDateTime(TimeOnly.MinValue), Zone.GetUtcOffset(d.ToDateTime(TimeOnly.MinValue))).ToUniversalTime();
    public static TimeOnly Now() => TimeOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Zone).DateTime);
}
