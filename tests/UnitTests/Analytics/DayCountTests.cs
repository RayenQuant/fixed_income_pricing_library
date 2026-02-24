using System;
using FixedIncomePricingLibrary.Analytics.DayCount;
using FixedIncomePricingLibrary.Core.Enums;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Analytics;

public class DayCountTests
{
    [Theory]
    [InlineData("2025-01-01", "2025-01-31", 30)]
    [InlineData("2025-01-01", "2025-07-01", 181)]
    public void Actual360_DayCount_IsCorrect(string startStr, string endStr, int expectedDays)
    {
        var start = DateTime.Parse(startStr);
        var end = DateTime.Parse(endStr);
        var calculator = new Actual360Calculator();

        Assert.Equal(expectedDays, calculator.DayCount(start, end));
    }

    [Fact]
    public void Actual360_YearFraction_IsCorrect()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2026, 1, 1); // 365 days
        var calculator = new Actual360Calculator();

        Assert.Equal(365.0 / 360.0, calculator.YearFraction(start, end), 10);
    }

    [Fact]
    public void Thirty360_YearFraction_FullYear_IsOne()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2026, 1, 1);
        var calculator = new Thirty360Calculator();

        Assert.Equal(1.0, calculator.YearFraction(start, end));
    }

    [Theory]
    [InlineData("2025-02-01", "2025-03-01", 30)] // Feb treated as 30 days in 30/360? 
    // Actually standard US 30/360: 360(Y2-Y1) + 30(M2-M1) + (D2-D1)
    // 360(0) + 30(1) + (1-1) = 30. Correct.
    public void Thirty360_DayCount_IsCorrect(string startStr, string endStr, int expectedDays)
    {
        var start = DateTime.Parse(startStr);
        var end = DateTime.Parse(endStr);
        var calculator = new Thirty360Calculator();

        Assert.Equal(expectedDays, calculator.DayCount(start, end));
    }

    [Fact]
    public void ActualActual_LeapYear_IsCorrect()
    {
        var start = new DateTime(2024, 1, 1); // Leap year
        var end = new DateTime(2025, 1, 1);
        var calculator = new ActualActualCalculator();

        Assert.Equal(1.0, calculator.YearFraction(start, end));
    }
}
