using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Analytics.Calendar;
using FixedIncomePricingLibrary.Analytics.CashFlows;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using Moq;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Analytics;

public class CashFlowTests
{
    private readonly ICalendar _calendar;

    public CashFlowTests()
    {
        _calendar = new HolidayCalendar(new List<DateTime>());
    }

    [Fact]
    public void ScheduleGenerator_Annual_GeneratesCorrectDates()
    {
        var start = new DateTime(2020, 1, 1);
        var end = new DateTime(2025, 1, 1);

        var schedule = ScheduleGenerator.Generate(start, end, CouponFrequency.Annual, _calendar, BusinessDayConvention.Following);

        Assert.Equal(5, schedule.Count);
        Assert.Equal(new DateTime(2021, 1, 1), schedule[0]);
        Assert.Equal(new DateTime(2025, 1, 1), schedule[4]);
    }

    [Fact]
    public void FixedLegGenerator_GeneratesCorrectAmounts()
    {
        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2025, 1, 1);
        var generator = new FixedLegCashFlowGenerator();

        var cfs = generator.Generate(
            start, end, 0.05, CouponFrequency.Annual, 1000,
            Currency.USD, DayCountConvention.Thirty360, _calendar, BusinessDayConvention.Following)
            .ToList();

        Assert.Single(cfs);
        // 1000 * 0.05 * 1.0 + 1000 = 1050
        Assert.Equal(1050, cfs[0].Amount);
    }

    [Fact]
    public void AccruedInterest_HalfYear_IsCorrect()
    {
        var last = new DateTime(2025, 1, 1);
        var settlement = new DateTime(2025, 7, 1);

        var accrued = AccruedInterestCalculator.Calculate(
            last, settlement, 0.04, 1_000_000, DayCountConvention.Thirty360);

        // 1M * 0.04 * 0.5 = 20,000
        Assert.Equal(20000, accrued, 2);
    }
}
