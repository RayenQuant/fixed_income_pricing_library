using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Analytics.Calendar;
using FixedIncomePricingLibrary.Core.Enums;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Analytics;

public class CalendarTests
{
    private readonly HolidayCalendar _calendar;

    public CalendarTests()
    {
        // Define some fake holidays for testing
        var holidays = new List<DateTime>
        {
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 20)
        };
        _calendar = new HolidayCalendar(holidays);
    }

    [Theory]
    [InlineData("2025-01-01", false)] // Holiday
    [InlineData("2025-01-02", true)]  // Regular day
    [InlineData("2025-01-04", false)] // Saturday
    [InlineData("2025-01-05", false)] // Sunday
    public void IsBusinessDay_ReturnsCorrectValue(string dateStr, bool expected)
    {
        var date = DateTime.Parse(dateStr);
        Assert.Equal(expected, _calendar.IsBusinessDay(date));
    }

    [Fact]
    public void AdjustDate_Following_AdjustsCorrectly()
    {
        var holiday = new DateTime(2025, 1, 1); // Wednesday
        var expected = new DateTime(2025, 1, 2);

        var result = _calendar.AdjustDate(holiday, BusinessDayConvention.Following);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void AdjustDate_ModifiedFollowing_StaysInMonth()
    {
        // 2025-01-31 is a Friday. If it were a holiday, Following would go to Feb.
        // Let's force it.
        var holidays = new List<DateTime> { new DateTime(2025, 1, 31) };
        var calendar = new HolidayCalendar(holidays);

        var result = calendar.AdjustDate(new DateTime(2025, 1, 31), BusinessDayConvention.ModifiedFollowing);

        // 2025-01-30 is Thursday
        Assert.Equal(new DateTime(2025, 1, 30), result);
    }

    [Fact]
    public void AddBusinessDays_SkipsWeekendsAndHolidays()
    {
        var start = new DateTime(2025, 1, 17); // Friday
                                               // Sat 18 (skip), Sun 19 (skip), Mon 20 (holiday, skip), Tue 21 (day 1), Wed 22 (day 2)

        var result = _calendar.AddBusinessDays(start, 2);

        Assert.Equal(new DateTime(2025, 1, 22), result);
    }
}
