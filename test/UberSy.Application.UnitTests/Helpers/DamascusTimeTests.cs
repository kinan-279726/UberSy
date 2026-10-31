using System;
using UberSy.Application.Common.Helpers;
using Xunit;

namespace UberSy.Application.Tests.Common.Helpers;

/// <summary>
/// تعتمد هذه الاختبارات على أن سوريا على UTC+3 طوال السنة (بدون توقيت صيفي منذ 2022).
/// إن فشلت على جهاز قديم فالسبب غالبًا قاعدة tzdata قديمة.
/// </summary>
public sealed class DamascusTimeTests
{
    [Theory]
    [InlineData(2026, 1, 15)]
    [InlineData(2026, 7, 15)]
    public void StartOfDayUtc_ShouldBeThreeHoursBeforeMidnightUtc(int year, int month, int day)
    {
        var date = new DateOnly(year, month, day);

        var result = DamascusTime.StartOfDayUtc(date);

        Assert.Equal(TimeSpan.Zero, result.Offset);
        Assert.Equal(date.ToDateTime(TimeOnly.MinValue).AddHours(-3), result.UtcDateTime);
    }

    [Fact]
    public void StartOfDayUtc_WhenNextDay_ShouldBeExactlyTwentyFourHoursLater()
    {
        var first = DamascusTime.StartOfDayUtc(new DateOnly(2026, 3, 10));
        var second = DamascusTime.StartOfDayUtc(new DateOnly(2026, 3, 11));

        Assert.Equal(TimeSpan.FromHours(24), second - first);
    }
}
